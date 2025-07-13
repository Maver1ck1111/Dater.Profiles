using Profiles.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiles.Application.RepositoriesContracts
{
    public interface IProfileRepository
    {
        Task<Result<Guid>> AddAsync(Profile profile);
        Task<Result<Profile>> GetByAccountIdAsync(Guid id);
        Task<Result<bool>> UpdateAsync(Profile profile);
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
