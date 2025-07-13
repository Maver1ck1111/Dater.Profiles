using Dapper;
using Microsoft.Extensions.Logging;
using Profiles.Application;
using Profiles.Application.RepositoriesContracts;
using Profiles.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Profiles.Infrastructure.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ILogger<ProfileRepository> _logger;
        private readonly ProfilesDbContext _dbContext;
        public ProfileRepository(ProfilesDbContext dbContext, ILogger<ProfileRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<Guid>> AddAsync(Profile profile)
        {
            if(profile == null)
            {
                _logger.LogError("Profile cant be empty");
                return Result<Guid>.Failure(400, "Profile cannot be null or empty");
            }

            if (profile.AccountID == Guid.Empty)
            {
                _logger.LogError("AccountID cannot be empty");
                return Result<Guid>.Failure(400, "AccountID cannot be empty");
            }

            Result<Profile> exsistingProfile = await GetByAccountIdAsync(profile.AccountID);

            if(exsistingProfile.Value != null)
            {
                _logger.LogError("Profile with the same AccountID already exists");
                return Result<Guid>.Failure(409, "Profile with the same AccountID already exists");
            }

            profile.ProfileID = Guid.NewGuid();

            string addQuery = "INSERT INTO \"Profiles\" (\"AccountID\", \"ProfileID\", \"ImagePaths\", \"Name\"," +
                " \"Description\", \"DateOfBirth\", \"BookInterest\", \"SportInterest\", \"MovieInterest\"," +
                " \"MusicInterest\", \"FoodInterest\", \"LifestyleInterest\", \"TravelInterest\")" +

                "VALUES(@AccountID, @ProfileID, @ImagePaths, @Name, @Description, @DateOfBirth, @BookInterest, @SportInterest, " +
                "@MovieInterest, @MusicInterest, @FoodInterest, @LifestyleInterest, @TravelInterest)";

            int rows = await _dbContext.DbConnection.ExecuteAsync(addQuery, profile);

            if(rows == 0)
            {
                _logger.LogError("Failed to add profile to the database");
                return Result<Guid>.Failure(500, "Failed to add profile to the database");
            }

            _logger.LogInformation("Profile with AccountID {AccountID} successfully added", profile.AccountID);

            return Result<Guid>.Success(profile.AccountID);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Profile ID cannot be empty");
                return Result<bool>.Failure(400, "Profile ID cannot be empty");
            }

            Result<Profile> exsistingProfile = await GetByAccountIdAsync(id);

            if(exsistingProfile.Value == null)
            {
                _logger.LogError("Profile with the given ID does not exist");
                return Result<bool>.Failure(404, "Profile with the given ID does not exist");
            }

            string deleteQuery = "DELETE FROM \"Profiles\" WHERE \"AccountID\" = @AccountID";

            int rows = await _dbContext.DbConnection.ExecuteAsync(deleteQuery, new { AccountID = id });

            if(rows == 0)
            {
                _logger.LogError("Failed to delete profile from the database");
                return Result<bool>.Failure(500, "Failed to delete profile from the database");
            }

            _logger.LogInformation("Profile with AccountID {AccountID} successfully deleted", id);

            return Result<bool>.Success(true);
        }

        public async Task<Result<Profile>> GetByAccountIdAsync(Guid id)
        {
            if(id == Guid.Empty)
            {
                _logger.LogError("Profile ID cannot be empty");
                return Result<Profile>.Failure(400, "Profile ID cannot be empty");
            }

            string getQuery = "SELECT * FROM \"Profiles\" WHERE \"AccountID\" = @AccountID";

            Profile? exsistingProfile = await _dbContext.DbConnection.QueryFirstOrDefaultAsync<Profile>(getQuery, new { AccountID = id });

            if (exsistingProfile == null)
            {
                _logger.LogError("Profile with the given ID does not exist");
                return Result<Profile>.Failure(404, "Profile with the given ID does not exist");
            }

            _logger.LogInformation("Profile with AccountID {AccountID} successfully retrieved", id);

            return Result<Profile>.Success(exsistingProfile);
        }

        public async Task<Result<bool>> UpdateAsync(Profile profile)
        {
            if (profile == null)
            {
                _logger.LogError("Profile cant be empty");
                return Result<bool>.Failure(400, "Profile cannot be null or empty");
            }

            if (profile.AccountID == Guid.Empty)
            {
                _logger.LogError("AccountID cannot be empty");
                return Result<bool>.Failure(400, "AccountID cannot be empty");
            }

            Result<Profile> exsistingProfile = await GetByAccountIdAsync(profile.AccountID);

            if(exsistingProfile.Value == null)
            {
                _logger.LogError("Profile with the given AccountID does not exist");
                return Result<bool>.Failure(404, "Profile with the given AccountID does not exist");
            }

            string updateQuery = "UPDATE \"Profiles\" SET \"ImagePaths\" = @ImagePaths, \"Name\" = @Name, " +
                "\"Description\" = @Description, \"DateOfBirth\" = @DateOfBirth, \"BookInterest\" = @BookInterest, " +
                "\"SportInterest\" = @SportInterest, \"MovieInterest\" = @MovieInterest, \"MusicInterest\" = @MusicInterest, " +
                "\"FoodInterest\" = @FoodInterest, \"LifestyleInterest\" = @LifestyleInterest, \"TravelInterest\" = @TravelInterest " +
                "WHERE \"AccountID\" = @AccountID";

            int rows = await _dbContext.DbConnection.ExecuteAsync(updateQuery, profile);

            if(rows == 0)
            {
                _logger.LogError("Failed to update profile in the database");
                return Result<bool>.Failure(500, "Failed to update profile in the database");
            }

            _logger.LogInformation("Profile with AccountID {AccountID} successfully updated", profile.AccountID);

            return Result<bool>.Success(true, 200);
        }
    }
}
