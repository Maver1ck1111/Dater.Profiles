using Profiles.Application.DTOs;
using Profiles.Domain;

namespace Profiles.Application.ServicesContracts
{
    public interface IProfileService
    {
        Task<Result<Guid>> AddProfileAsync(ProfileRequestDTO profileRequest);
        Task<Result<Profile>> GetProfileByAccountIdAsync(Guid accountId);
        Task<Result<bool>> UpdateProfileAsync(ProfileRequestDTO profileUpdateRequest);
        Task<Result<bool>> DeleteProfileAsync(Guid profileId);
    }
}
