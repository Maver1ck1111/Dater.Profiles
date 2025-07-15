using AutoMapper;
using Microsoft.Extensions.Logging;
using Profiles.Application.DTOs;
using Profiles.Application.RepositoriesContracts;
using Profiles.Application.ServicesContracts;
using Profiles.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Result<Guid>> AddProfileAsync(ProfileRequestDTO profileRequest)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> DeleteProfileAsync(Guid profileId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Profiles.Domain.Profile>> GetProfileByAccountIdAsync(Guid accountId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> UpdateProfileAsync(ProfileRequestDTO profileUpdateRequest)
        {
            throw new NotImplementedException();
        }
    }
}
