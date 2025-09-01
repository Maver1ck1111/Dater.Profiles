using Profiles.Application.DTOs;
using Profiles.Domain;

namespace Profiles.Application.ServicesContracts
{
    public interface IProfileService
    {
        Task<Result<Guid>> AddProfileAsync(Profiles.Domain.Profile profileRequest);
        Task<Result<Profile>> GetProfileByAccountIdAsync(Guid accountId);
        Task<Result<bool>> UpdateProfileAsync(Profiles.Domain.Profile profileUpdateRequest);
        Task<Result<bool>> DeleteProfileAsync(Guid profileId);
        Task<Result<IEnumerable<Profile>>> GetProfilesByFilterAsync(IEnumerable<Guid> guids, string searchGender, int limit = 1000);
    }
}
