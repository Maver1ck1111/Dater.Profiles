using Dapper;
using Npgsql;
using System.Data;

namespace Profiles.Infrastructure
{
    public class ProfilesDbContext
    {
        public IDbConnection DbConnection { get; private set; }
        public ProfilesDbContext() 
        {
            string connectionString = $"Host={Environment.GetEnvironmentVariable("Host")}; " +
                $"Port={Environment.GetEnvironmentVariable("Port")}; " +
                $"Database={Environment.GetEnvironmentVariable("DatabaseName")}; " +
                $"Username={Environment.GetEnvironmentVariable("Username")}; " +
                $"Password={Environment.GetEnvironmentVariable("Password")};";

            DbConnection = new NpgsqlConnection(connectionString);
        }
    }
}
