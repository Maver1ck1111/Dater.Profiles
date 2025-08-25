using Profiles.Domain;


namespace Profiles.Application.RepositoriesContracts
{
    public interface IProfileRepository
    {
        Task<Result<Guid>> AddAsync(Profile profile);
        Task<Result<Profile>> GetByAccountIdAsync(Guid id);
        Task<Result<bool>> UpdateAsync(Profile profile);
        Task<Result<bool>> DeleteAsync(Guid id);
        Task<Result<IEnumerable<Profile>>> GetProfilesByFilterAsync(IEnumerable<Guid> guids, int limit = 1000);
    }
}
