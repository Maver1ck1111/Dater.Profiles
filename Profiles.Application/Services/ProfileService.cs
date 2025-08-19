using AutoMapper;
using Microsoft.Extensions.Logging;
using Profiles.Application.RepositoriesContracts;
using Profiles.Application.ServicesContracts;

namespace Profiles.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ILogger<ProfileService> _logger;
        private readonly IMapper _mapper;
        public ProfileService(IProfileRepository profileRepository, ILogger<ProfileService> logger, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> AddProfileAsync(Profiles.Domain.Profile profileRequest)
        {
            if (profileRequest == null)
            {
                _logger.LogError("Profile can not be null");
                return Result<Guid>.Failure(400, "Profile cannot be null or empty");
            }

            if (profileRequest.AccountID == Guid.Empty)
            {
                _logger.LogError("AccountID cannot be empty");
                return Result<Guid>.Failure(400, "AccountID cannot be empty");
            }

            Profiles.Domain.Profile profile = _mapper.Map<Profiles.Domain.Profile>(profileRequest);

            Result<Guid> result = await _profileRepository.AddAsync(profile);

            if(result.StatusCode == 409)
            {
                _logger.LogError("Account already exist");
                return Result<Guid>.Failure(409, "Profile with the same AccountID already exists");
            }

            if (result.StatusCode != 200)
            {
                _logger.LogError("Failed to add profile: {ErrorMessage}", result.ErrorMessage);
                return Result<Guid>.Failure(result.StatusCode, result.ErrorMessage);
            }

            _logger.LogInformation("Profile created successfully with ID: {ProfileId}", result.Value);

            return Result<Guid>.Success(result.Value);
        }

        public async Task<Result<bool>> DeleteProfileAsync(Guid profileId)
        {
            if(profileId == Guid.Empty)
            {
                _logger.LogError("Profile ID cannot be empty");
                return Result<bool>.Failure(400, "Profile ID cannot be empty");
            }

            Result<Profiles.Domain.Profile> existingProfile = await _profileRepository.GetByAccountIdAsync(profileId);

            if (existingProfile.StatusCode == 404)
            {
                _logger.LogError("Profile with the given ID does not exist");
                return Result<bool>.Failure(404, "Profile not found");
            }

            if (existingProfile.StatusCode != 200)
            {
                _logger.LogError("Failed to delete profile: {ErrorMessage}", existingProfile.ErrorMessage);
                return Result<bool>.Failure(existingProfile.StatusCode, existingProfile.ErrorMessage);
            }

            foreach (var path in existingProfile.Value.ImagePaths)
            {
                if (string.IsNullOrWhiteSpace(path))
                    continue;

                var fullPath = Path.Combine("ProfilePics", path);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            Result<bool> isDeleted = await _profileRepository.DeleteAsync(profileId);

            if (isDeleted.StatusCode != 200)
            {
                _logger.LogError("Failed to delete profile: {ErrorMessage}", isDeleted.ErrorMessage);
                return Result<bool>.Failure(isDeleted.StatusCode, isDeleted.ErrorMessage);
            }

            _logger.LogInformation("Profile with ID {ProfileId} deleted successfully", profileId);

            return Result<bool>.Success(true);
        }

        public async Task<Result<Profiles.Domain.Profile>> GetProfileByAccountIdAsync(Guid accountId)
        {
            if(accountId == Guid.Empty)
            {
                _logger.LogError("Account ID cannot be empty");
                return Result<Profiles.Domain.Profile>.Failure(400, "Account ID cannot be empty");
            }

            Result<Profiles.Domain.Profile> result = await _profileRepository.GetByAccountIdAsync(accountId);
            
            if(result.StatusCode == 404)
            {
                _logger.LogError("Account with id: {id} not found", accountId);
                return Result<Profiles.Domain.Profile>.Failure(404, "Profile not found");
            }

            if(result.StatusCode != 200)
            {
                _logger.LogError("Failed to retrieve profile: {ErrorMessage}", result.ErrorMessage);
                return Result<Profiles.Domain.Profile>.Failure(result.StatusCode, result.ErrorMessage);
            }

            _logger.LogInformation("Profile retrieved successfully for account ID: {AccountId}", accountId);
            return Result<Profiles.Domain.Profile>.Success(result.Value);
        }

        public async Task<Result<bool>> UpdateProfileAsync(Profiles.Domain.Profile profileUpdateRequest)
        {
            if(profileUpdateRequest == null)
            {
                _logger.LogError("Profile update request can not be null");
                return Result<bool>.Failure(400, "Profile update request is invalid");
            }

            if (profileUpdateRequest.AccountID == Guid.Empty)
            {
                _logger.LogError("AccountID cannot be empty");
                return Result<bool>.Failure(400, "AccountID cannot be empty");
            }

            Profiles.Domain.Profile profile = _mapper.Map<Profiles.Domain.Profile>(profileUpdateRequest);

            Result<bool> result = await _profileRepository.UpdateAsync(profile);

            if (result.StatusCode == 404)
            {
                _logger.LogError("Profile with the given AccountID does not exist");
                return Result<bool>.Failure(404, "Profile with the given AccountID does not exist");
            }

            if (result.StatusCode != 200)
            {
                _logger.LogError("Failed to update profile: {ErrorMessage}", result.ErrorMessage);
                return Result<bool>.Failure(result.StatusCode, result.ErrorMessage);
            }

            _logger.LogInformation("Profile with AccountID {AccountID} successfully updated", profile.AccountID);
            return Result<bool>.Success(true);
        }
    }
}
