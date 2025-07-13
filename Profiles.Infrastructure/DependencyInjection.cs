using Microsoft.Extensions.DependencyInjection;
using Profiles.Application.RepositoriesContracts;
using Profiles.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiles.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<ProfilesDbContext>();

            return services;
        }
    }
}
