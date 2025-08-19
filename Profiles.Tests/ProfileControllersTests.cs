using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Profiles.Application;
using Profiles.Application.RepositoriesContracts;
using Profiles.Domain;
using Profiles.Domain.Enums;
using Profiles.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Profiles.Application.ServicesContracts;
using Profiles.Application.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Profiles.Application.Mappers;

namespace Profiles.Tests
{
    public class ProfileControllersTests
    {
        private readonly Mock<ILogger<ProfileController>> _logger = new Mock<ILogger<ProfileController>>();
        private readonly Mock<IProfileService> _profileService = new Mock<IProfileService>();
        private readonly IMapper _mapper;

        public ProfileControllersTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProfileRequestDTOToProfile>();
            }, NullLoggerFactory.Instance);

            config.AssertConfigurationIsValid();

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task CreateProfile_ShouldReturnOkResult()
        {
            Guid accountID = Guid.NewGuid();

            _profileService.Setup(repo => repo.AddProfileAsync(It.IsAny<Profiles.Domain.Profile>()))
                .ReturnsAsync(Result<Guid>.Success(accountID));

            var controller = new ProfileController(_profileService.Object, _logger.Object, _mapper);

            var result = await controller.CreateProfile(CreateFactory.CreateDTOTestFactory());

            result.Result.Should().BeOfType<OkObjectResult>();

            var value = (result.Result as OkObjectResult)?.Value;

            value.Should().BeOfType<Guid>();
            value.Should().Be(accountID);
        }

        [Fact]
        public async Task CreateProfile_ShouldReturn409_ConflictWhenProfileAlreadyExists()
        {
            _profileService.Setup(repo => repo.AddProfileAsync(It.IsAny<Profiles.Domain.Profile>()))
                .ReturnsAsync(Result<Guid>.Failure(409, "Profile already exist"));

            var controller = new ProfileController(_profileService.Object, _logger.Object, _mapper);

            var result = await controller.CreateProfile(CreateFactory.CreateDTOTestFactory());

            result.Result.Should().BeOfType<ConflictObjectResult>();

            var value = (result.Result as ConflictObjectResult)?.Value;

            value.Should().Be("Profile already exist");
        }

        [Fact]
        public async Task GetProfile_ShouldReturnOkResult()
        {
            Guid accountID = Guid.NewGuid();

            var profile = CreateFactory.CreateTestFactory();
            profile.AccountID = accountID;

            _profileService.Setup(repo => repo.GetProfileByAccountIdAsync(accountID))
                .ReturnsAsync(Result<Profiles.Domain.Profile>.Success(profile));

            var controller = new ProfileController(_profileService.Object, _logger.Object, _mapper);

            var result = await controller.GetProfileByAccountId(accountID);

            result.Result.Should().BeOfType<OkObjectResult>();

            var value = (result.Result as OkObjectResult)?.Value;

            value.Should().BeOfType<Profiles.Domain.Profile>();
            value.Should().BeEquivalentTo(profile, options => options.Excluding(x => x.ProfileID));
        }

        [Fact]
        public async Task GetProfile_ShouldReturn404_InccorectAccountId()
        {
            Guid accountID = Guid.NewGuid();

            _profileService.Setup(repo => repo.GetProfileByAccountIdAsync(accountID))
                .ReturnsAsync(Result<Profiles.Domain.Profile>.Failure(404, "Profile not found"));

            var controller = new ProfileController(_profileService.Object, _logger.Object, _mapper);

            var result = await controller.GetProfileByAccountId(accountID);

            result.Result.Should().BeOfType<NotFoundObjectResult>();

            var value = (result.Result as NotFoundObjectResult)?.Value;

            value.Should().Be("Profile not found");
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnOkResult()
        {
            _profileService.Setup(repo => repo.UpdateProfileAsync(It.IsAny<Profiles.Domain.Profile>()))
                .ReturnsAsync(Result<bool>.Success(true));

            var controller = new ProfileController(_profileService.Object, _logger.Object, _mapper);

            var profile = CreateFactory.CreateDTOTestFactory();

            var result = await controller.UpdateProfile(profile);

            result.Result.Should().BeOfType<OkObjectResult>();

            var value = (result.Result as OkObjectResult)?.Value;

            value.Should().BeOfType<bool>();
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturn404_InccorectAccountId()
        {
            _profileService.Setup(repo => repo.UpdateProfileAsync(It.IsAny<Profiles.Domain.Profile>()))
                .ReturnsAsync(Result<bool>.Failure(404, "Profile not found"));


            var controller = new ProfileController(_profileService.Object, _logger.Object, _mapper);

            var result = await controller.UpdateProfile(CreateFactory.CreateDTOTestFactory());

            result.Result.Should().BeOfType<NotFoundObjectResult>();

            var value = (result.Result as NotFoundObjectResult)?.Value;

            value.Should().Be("Profile not found");
        }

        [Fact]
        public async Task DeleteProfile_ShouldReturnOkResult()
        {
            Guid profileId = Guid.NewGuid();

            _profileService.Setup(repo => repo.DeleteProfileAsync(profileId))
                .ReturnsAsync(Result<bool>.Success(true));

            var controller = new ProfileController(_profileService.Object, _logger.Object, _mapper);

            var result = await controller.DeleteProfile(profileId);

            result.Result.Should().BeOfType<OkObjectResult>();

            var value = (result.Result as OkObjectResult)?.Value;

            value.Should().BeOfType<bool>();
        }

        [Fact]
        public async Task DeleteProfile_ShouldReturn404_InccorectProfileId()
        {
            Guid profileId = Guid.NewGuid();

            _profileService.Setup(repo => repo.DeleteProfileAsync(profileId))
                .ReturnsAsync(Result<bool>.Failure(404, "Profile not found"));

            var controller = new ProfileController(_profileService.Object, _logger.Object, _mapper);

            var result = await controller.DeleteProfile(profileId);

            result.Result.Should().BeOfType<NotFoundObjectResult>();

            var value = (result.Result as NotFoundObjectResult)?.Value;

            value.Should().Be("Profile not found");
        }
    }
}
