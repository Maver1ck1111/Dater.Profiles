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

namespace Profiles.Tests
{
    public class ProfileControllersTests
    {
        private readonly Mock<ILogger<ProfileController>> _logger = new Mock<ILogger<ProfileController>>();
        private readonly Mock<IProfileRepository> _profileRepository = new Mock<IProfileRepository>();
        private Profile CreateTestFactory() => new Profile()
        {
            Name = "Test User",
            Description = "Test Description",
            DateOfBirth = new DateTime(1990, 1, 1),
            AccountID = Guid.NewGuid(),
            BookInterest = BookInterest.Fantasy,
            MovieInterest = MovieInterest.Thriller,
            MusicInterest = MusicInterest.Rock,
            ImagePaths = new string[] { "path/to/image1.jpg", "path/to/image2.jpg" },
        };

        [Fact]
        public async Task CreateProfile_ShouldReturnOkResult()
        {
            var controller = new ProfileController(_profileRepository.Object, _logger.Object);

            Guid accountID = Guid.NewGuid();

            _profileRepository.Setup(repo => repo.AddAsync(It.IsAny<Profiles.Domain.Profile>()))
                .ReturnsAsync(Result<Guid>.Success(accountID));

            var result = await controller.CreateProfile(CreateTestFactory());

            result.Result.Should().BeOfType<OkObjectResult>();

            var value = (result.Result as OkObjectResult)?.Value;

            value.Should().BeOfType<Guid>();
            value.Should().Be(accountID);
        }

        [Fact]
        public async Task CreateProfile_ShouldReturn409_ConflictWhenProfileAlreadyExists()
        {
            var controller = new ProfileController(_profileRepository.Object, _logger.Object);

            _profileRepository.Setup(repo => repo.AddAsync(It.IsAny<Profile>()))
                .ReturnsAsync(Result<Guid>.Failure(409, "Profile already exist"));

            var result = await controller.CreateProfile(CreateTestFactory());

            result.Result.Should().BeOfType<ConflictObjectResult>();

            var value = (result.Result as ConflictObjectResult)?.Value;

            value.Should().Be("Profile already exist");
        }

        [Fact]
        public async Task GetProfile_ShouldReturnOkResult()
        {
            var controller = new ProfileController(_profileRepository.Object, _logger.Object);

            Guid accountID = Guid.NewGuid();
            var profile = CreateTestFactory();
            profile.AccountID = accountID;

            _profileRepository.Setup(repo => repo.GetByAccountIdAsync(accountID))
                .ReturnsAsync(Result<Profile>.Success(profile));

            var result = await controller.GetProfileByAccountId(accountID);

            result.Result.Should().BeOfType<OkObjectResult>();

            var value = (result.Result as OkObjectResult)?.Value;

            value.Should().BeOfType<Profile>();
            value.Should().BeEquivalentTo(profile, options => options.Excluding(x => x.ProfileID));
        }

        [Fact]
        public async Task GetProfile_ShouldReturn404_InccorectAccountId()
        {
            var controller = new ProfileController(_profileRepository.Object, _logger.Object);

            Guid accountID = Guid.NewGuid();

            _profileRepository.Setup(repo => repo.GetByAccountIdAsync(accountID))
                .ReturnsAsync(Result<Profile>.Failure(404, "Profile not found"));

            var result = await controller.GetProfileByAccountId(accountID);

            result.Result.Should().BeOfType<NotFoundObjectResult>();

            var value = (result.Result as NotFoundObjectResult)?.Value;

            value.Should().Be("Profile not found");
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnOkResult()
        {
            var controller = new ProfileController(_profileRepository.Object, _logger.Object);

            var profile = CreateTestFactory();

            _profileRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Profile>()))
                .ReturnsAsync(Result<bool>.Success(true));

            var result = await controller.UpdateProfile(profile);

            result.Result.Should().BeOfType<OkObjectResult>();

            var value = (result.Result as OkObjectResult)?.Value;

            value.Should().BeOfType<bool>();
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturn404_InccorectAccountId()
        {
            var controller = new ProfileController(_profileRepository.Object, _logger.Object);

            _profileRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Profile>()))
                .ReturnsAsync(Result<bool>.Failure(404, "Profile not found"));

            var result = await controller.UpdateProfile(CreateTestFactory());

            result.Result.Should().BeOfType<NotFoundObjectResult>();

            var value = (result.Result as NotFoundObjectResult)?.Value;

            value.Should().Be("Profile not found");
        }

        [Fact]
        public async Task DeleteProfile_ShouldReturnOkResult()
        {
            var controller = new ProfileController(_profileRepository.Object, _logger.Object);

            Guid profileId = Guid.NewGuid();

            _profileRepository.Setup(repo => repo.DeleteAsync(profileId))
                .ReturnsAsync(Result<bool>.Success(true));

            var result = await controller.DeleteProfile(profileId);

            result.Result.Should().BeOfType<OkObjectResult>();

            var value = (result.Result as OkObjectResult)?.Value;

            value.Should().BeOfType<bool>();
        }

        [Fact]
        public async Task DeleteProfile_ShouldReturn404_InccorectProfileId()
        {
            var controller = new ProfileController(_profileRepository.Object, _logger.Object);

            Guid profileId = Guid.NewGuid();

            _profileRepository.Setup(repo => repo.DeleteAsync(profileId))
                .ReturnsAsync(Result<bool>.Failure(404, "Profile not found"));

            var result = await controller.DeleteProfile(profileId);

            result.Result.Should().BeOfType<NotFoundObjectResult>();

            var value = (result.Result as NotFoundObjectResult)?.Value;

            value.Should().Be("Profile not found");
        }
    }
}
