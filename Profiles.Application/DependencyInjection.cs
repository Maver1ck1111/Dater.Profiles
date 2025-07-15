using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Profiles.Application.Validators;
using FluentValidation.AspNetCore;
using Profiles.Application.Mappers;

namespace Profiles.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<ProfileRequestDTOValidator>();
            services.AddFluentValidationAutoValidation();

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ProfileRequestDTOToProfile>();
            });

            return services;
        }
    }
}
