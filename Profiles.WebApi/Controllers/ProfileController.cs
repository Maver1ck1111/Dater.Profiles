using Microsoft.AspNetCore.Mvc;
using Profiles.Application;
using Profiles.Application.DTOs;
using Profiles.Application.RepositoriesContracts;
using Profiles.Domain;

namespace Profiles.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ILogger<ProfileController> _logger;
        public ProfileController(IProfileRepository profileRepository, ILogger<ProfileController> logger)
        {
            _profileRepository = profileRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProfile(Profile request)
        {
            _logger.LogInformation("Creating profile for account ID: {AccountId}", request.AccountID);

            Result<Guid> result = await _profileRepository.AddAsync(request);

            if (result.StatusCode == 409)
            {
                _logger.LogError("Profile creation failed: {ErrorMessage}", result.ErrorMessage);
                return Conflict(result.ErrorMessage);
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
            if(accountId == Guid.Empty)
            {
                _logger.LogError("Account ID cannot be empty");
                return BadRequest("Account ID cannot be empty");
            }

            _logger.LogInformation("Retrieving profile for account ID: {AccountId}", accountId);

            Result<Profile> result = await _profileRepository.GetByAccountIdAsync(accountId);

            if(result.StatusCode == 404)
            {
                _logger.LogWarning("Profile not found for account ID: {AccountId}", accountId);
                return NotFound("Profile not found");
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
        public async Task<ActionResult<bool>> UpdateProfile(Profile request)
        {
            _logger.LogInformation("Updating profile with {id}", request.AccountID);

            Result<bool> result = await _profileRepository.UpdateAsync(request);

            if(result.StatusCode == 404)
            {
                _logger.LogWarning("Profile not found for account ID: {AccountId}", request.AccountID);
                return NotFound("Profile not found");
            }

            if(result.StatusCode != 200)
            {
                _logger.LogError("Failed to update profile: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            _logger.LogInformation("Profile updated successfully for account ID: {AccountId}", request.AccountID);

            return Ok(result.Value);
        }

        [HttpDelete("{profileId}")]
        public async Task<ActionResult<bool>> DeleteProfile(Guid profileId)
        {
            if(profileId == Guid.Empty)
            {
                _logger.LogError("Profile ID cannot be empty");
                return BadRequest("Profile ID cannot be empty");
            }

            _logger.LogInformation("Deleting profile with ID: {ProfileId}", profileId);

            Result<bool> result = await _profileRepository.DeleteAsync(profileId);

            if(result.StatusCode == 404)
            {
                _logger.LogWarning("Profile not found for ID: {ProfileId}", profileId);
                return NotFound("Profile not found");
            }

            if(result.StatusCode != 200)
            {
                _logger.LogError("Failed to delete profile: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            return Ok(result.Value);
        }
    }
}
