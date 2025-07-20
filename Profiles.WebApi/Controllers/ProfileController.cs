using Microsoft.AspNetCore.Mvc;
using Profiles.Application;
using Profiles.Application.DTOs;
using Profiles.Application.RepositoriesContracts;
using Profiles.Application.ServicesContracts;
using Profiles.Domain;
using System.IO;

namespace Profiles.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileRepository;
        private readonly ILogger<ProfileController> _logger;
        public ProfileController(IProfileService profileService, ILogger<ProfileController> logger)
        {
            _profileRepository = profileService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProfile(ProfileRequestDTO request)
        {
            _logger.LogInformation("Creating profile for account ID: {AccountId}", request.AccountID);

            Result<Guid> result = await _profileRepository.AddProfileAsync(request);

            if(result.StatusCode == 409)
            {
                _logger.LogError("Profile already exists for account ID: {AccountId}", request.AccountID);
                return Conflict(result.ErrorMessage);
            }

            if(result.StatusCode == 400)
            {
                _logger.LogError("Profile is not in correct format");
                return BadRequest(result.ErrorMessage);
            }

            if(result.StatusCode != 200)
            {
                _logger.LogError("Failed to create profile: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            _logger.LogInformation("Profile created successfully with ID: {ProfileId}", result.Value);

            return Ok(result.Value);
        }

        [HttpGet("{accountId}")]
        public async Task<ActionResult<Profile>> GetProfileByAccountId(Guid accountId)
        {
            Result<Profile> result = await _profileRepository.GetProfileByAccountIdAsync(accountId);

            if(result.StatusCode == 404)
            {
                _logger.LogError("Account with id: {id} not found", accountId);
                return NotFound(result.ErrorMessage);
            }

            if(result.StatusCode != 200)
            {
                _logger.LogError("Failed to retrieve profile: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            _logger.LogInformation("Profile retrieved successfully for account ID: {AccountId}", accountId);
            return Ok(result.Value);
        }

        [HttpPut]
        public async Task<ActionResult<bool>> UpdateProfile(ProfileRequestDTO request)
        {
            _logger.LogInformation("Updating profile for account ID: {AccountId}", request.AccountID);

            Result<bool> result = await _profileRepository.UpdateProfileAsync(request);

            if(result.StatusCode == 400)
            {
                _logger.LogError("Profile update request is invalid");
                return BadRequest(result.ErrorMessage);
            }

            if(result.StatusCode == 404)
            {
                _logger.LogError("Profile with account ID: {AccountId} not found", request.AccountID);
                return NotFound(result.ErrorMessage);
            }

            if(result.StatusCode != 200)
            {
                _logger.LogError("Failed to update profile: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            _logger.LogInformation("Profile updated successfully for account ID: {AccountId}", request.AccountID);
            return Ok(result.Value);
        }

        [HttpPost("set-photo/{accountID}")]
        public async Task<IActionResult> SetPhoto(Guid accountID, [FromForm] IEnumerable<IFormFile> photos)
        {
            if (accountID == Guid.Empty)
            {
                _logger.LogError("Account ID cannot be empty");
                return BadRequest("Account ID cannot be empty");
            }

            if (!photos.Any())
            {
                _logger.LogError("Must be at least 1 photo");
                return BadRequest("Account ID cannot be empty");
            }

            if(photos.Count() > 3)
            {
                _logger.LogError("Cannot upload more than 3 photos");
                return BadRequest("Cannot upload more than 3 photos");
            }

            Result<Profile> getResult = await _profileRepository.GetProfileByAccountIdAsync(accountID);

            if(getResult.StatusCode == 404)
            {
                _logger.LogError("Profile with id: {id} not found", accountID);
                return NotFound("Profile not found");
            }

            if(getResult.StatusCode != 200)
            {
                _logger.LogError("Failed to retrieve profile: {ErrorMessage}", getResult.ErrorMessage);
                return StatusCode(getResult.StatusCode, getResult.ErrorMessage);
            }

            string baseDir = AppContext.BaseDirectory;
            string root = Path.GetFullPath(Path.Combine(baseDir, "../../../../"));
            string directory = Path.Combine(root, "ProfilePics");

            int counter = 0;

            foreach (var photo in photos)
            {
                if (photo == null || photo.Length == 0)
                    continue;

                var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                if (!allowedExtensions.Contains(extension))
                    continue;

                var fileName = $"{getResult.Value.AccountID}.{counter}.{extension}";

                var fullPath = Path.Combine(directory, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                getResult.Value.ImagePaths[counter++] = fileName;
            }

            _logger.LogInformation("Setting photo for account ID: {AccountId}", accountID);

            return Ok();
        }

        [HttpDelete("{profileId}")]
        public async Task<ActionResult<bool>> DeleteProfile(Guid profileId)
        {
            _logger.LogInformation("Deleting profile with id: {id}", profileId);

            Result<bool> result = await _profileRepository.DeleteProfileAsync(profileId);

            if(result.StatusCode == 404)
            {
               _logger.LogError("Profile with id: {id} not found", profileId);
                return NotFound(result.ErrorMessage);
            }

            if(result.StatusCode != 200)
            {
                _logger.LogError("Failed to delete profile: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }


            _logger.LogInformation("Profile with id: {id} deleted successfully", profileId);
            return Ok(result.Value);
        }

        [HttpGet("/getPhotoByID/{accountID}/{index}")]
        public async Task<IActionResult> GetPhotos(Guid accountID, int index)
        {
            _logger.LogInformation("Getting photos by id: {id}", accountID);

            Result<Profile> profileResult = await _profileRepository.GetProfileByAccountIdAsync(accountID);

            if (profileResult.StatusCode == 404)
            {
                _logger.LogError("Profile with id: {id} not found", accountID);
                return NotFound("Profile not found");
            }

            string baseDir = AppContext.BaseDirectory; 
            string root = Path.GetFullPath(Path.Combine(baseDir, "../../../../"));
            string folder = Path.Combine(root, "ProfilePics");

            if (!Directory.Exists(folder))
            {
                _logger.LogError("Profile pictures directory does not exist");
                return NotFound("Profile pictures directory does not exist");
            }

            var files = Directory.GetFiles(folder, $"{accountID}.{index}.*");

            if (!files.Any())
            {
                return NotFound("No photos");
            }

            var filePath = files[0];
            var extension = Path.GetExtension(filePath).TrimStart('.').ToLowerInvariant();
            var contentType = $"image/{extension}";

            return PhysicalFile(filePath, contentType);
        }
    }
}
