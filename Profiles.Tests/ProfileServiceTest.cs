using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Profiles.Application;
using Profiles.Application.DTOs;
using Profiles.Application.RepositoriesContracts;
using Profiles.Application.Services;
using Profiles.Domain;

namespace Profiles.Tests
{
    public class ProfileServiceTest
    {
        private readonly Mock<ILogger<ProfileService>> _mockLogger = new Mock<ILogger<ProfileService>>();
        private readonly Mock<IProfileRepository> _repository = new Mock<IProfileRepository>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();

        [Fact]
        public async Task AddProfile_ShouldReturnCorrectResponse()
        {
            _repository
                .Setup(repo => repo.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<Profile>.Failure(404, "Not found"));

            _repository
                .Setup(repo => repo.AddAsync(It.IsAny<Profile>()))
                .ReturnsAsync(Result<Guid>.Success(Guid.NewGuid()));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object);

            ProfileRequestDTO profileRequestDTO = CreateFactory.CreateDTOTestFactory();

            var addResult = await profileService.AddProfileAsync(profileRequestDTO);

            addResult.StatusCode.Should().Be(200);
            addResult.Value.Should().NotBeEmpty();
            addResult.Value.Should().NotBe(Guid.Empty);

            var getResult = await profileService.GetProfileByAccountIdAsync(profileRequestDTO.AccountID);
            getResult.StatusCode.Should().Be(200);
            getResult.Value.Should().NotBeNull();
            getResult.Value.Should().BeEquivalentTo(profileRequestDTO, options => options.Excluding(x => x.Images));

            getResult.Value.ImagePaths.Should().NotBeNullOrEmpty();

            foreach (var item in getResult.Value.ImagePaths)
            {
                item.Should().Contain(profileRequestDTO.AccountID.ToString());
            };
        }

        [Fact]
        public async Task AddProfile_ShouldReturn409_ThisProfileAlreadyExist()
        {
            _repository
                .Setup(repo => repo.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<Profile>.Success(new Profile()));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object);

            ProfileRequestDTO profileRequestDTO = CreateFactory.CreateDTOTestFactory();

            var addResult = await profileService.AddProfileAsync(profileRequestDTO);

            addResult.StatusCode.Should().Be(409);
        }

        [Fact]
        public async Task DeleteProfile_ShouldReturnCorrectResponse()
        {
            Guid profileId = Guid.NewGuid();

            _repository
                .Setup(repo => repo.DeleteAsync(profileId))
                .ReturnsAsync(Result<bool>.Success(true));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object);

            var deleteResult = await profileService.DeleteProfileAsync(profileId);

            deleteResult.StatusCode.Should().Be(200);
            deleteResult.Value.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteProfile_ShouldReturn404_ProfileNotFound()
        {
            Guid profileId = Guid.NewGuid();

            _repository
                .Setup(repo => repo.DeleteAsync(profileId))
                .ReturnsAsync(Result<bool>.Failure(404, "Profile not found"));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object);

            var deleteResult = await profileService.DeleteProfileAsync(profileId);

            deleteResult.StatusCode.Should().Be(404);
            deleteResult.Value.Should().BeFalse();
        }

        [Fact]
        public async Task GetProfileByAccountId_ShouldReturnCorrectResponse()
        {
            Profile profile = CreateFactory.CreateTestFactory();

            _repository
                .Setup(repo => repo.GetByAccountIdAsync(profile.AccountID))
                .ReturnsAsync(Result<Profile>.Success(profile));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object);

            var getResult = await profileService.GetProfileByAccountIdAsync(profile.AccountID);

            getResult.StatusCode.Should().Be(200);
            getResult.Value.Should().NotBeNull();
            getResult.Value.AccountID.Should().Be(profile.AccountID);
        }

        [Fact]
        public async Task GetProfileByAccountId_ShouldReturn404_ProfileNotFound()
        {
            Profile profile = CreateFactory.CreateTestFactory();

            _repository
                .Setup(repo => repo.GetByAccountIdAsync(profile.AccountID))
                .ReturnsAsync(Result<Profile>.Failure(404, "Profile not found"));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object);

            var getResult = await profileService.GetProfileByAccountIdAsync(profile.AccountID);

            getResult.StatusCode.Should().Be(404);
            getResult.Value.Should().BeNull();
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnCorrectResponse()
        {
            ProfileRequestDTO profile = CreateFactory.CreateDTOTestFactory();
            profile.DateOfBirth = new DateTime(2000, 5, 5);

            _repository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Profile>()))
                .ReturnsAsync(Result<bool>.Success(true));

            _repository
                .Setup(repo => repo.GetByAccountIdAsync(profile.AccountID))
                .ReturnsAsync(Result<Profile>.Success(new Profile()));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object);

            var updateResult = await profileService.UpdateProfileAsync(profile);

            updateResult.StatusCode.Should().Be(200);
            updateResult.Value.Should().BeTrue();

            var getResult = await profileService.GetProfileByAccountIdAsync(profile.AccountID);

            getResult.Value.Should().NotBeNull();
            getResult.Value.DateOfBirth.Should().Be(new DateTime(2000, 5, 5));
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturn404_ProfileNotFound()
        {
            ProfileRequestDTO profile = CreateFactory.CreateDTOTestFactory();

            _repository
                .Setup(repo => repo.GetByAccountIdAsync(profile.AccountID))
                .ReturnsAsync(Result<Profile>.Failure(404, "Profile not found"));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object);

            var updateResult = await profileService.UpdateProfileAsync(profile);

            updateResult.StatusCode.Should().Be(404);
            updateResult.Value.Should().BeFalse();
        }
    }
}
