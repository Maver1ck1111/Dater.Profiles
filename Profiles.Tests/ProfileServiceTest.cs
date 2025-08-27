using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Profiles.Application;
using Profiles.Application.DTOs;
using Profiles.Application.Mappers;
using Profiles.Application.RepositoriesContracts;
using Profiles.Application.Services;
using Profiles.Domain;

namespace Profiles.Tests
{
    public class ProfileServiceTest
    {
        private readonly Mock<ILogger<ProfileService>> _mockLogger = new Mock<ILogger<ProfileService>>();
        private readonly Mock<IProfileRepository> _repository = new Mock<IProfileRepository>();
        private readonly IMapper _mapper;

        public ProfileServiceTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProfileRequestDTOToProfile>();
            }, NullLoggerFactory.Instance);

            config.AssertConfigurationIsValid();

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task AddProfile_ShouldReturnCorrectResponse()
        {
            _repository
                .Setup(repo => repo.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<Profiles.Domain.Profile>.Failure(404, "Not found"));

            _repository
                .Setup(repo => repo.AddAsync(It.IsAny<Profiles.Domain.Profile>()))
                .ReturnsAsync(Result<Guid>.Success(Guid.NewGuid()));

            Profiles.Domain.Profile profile = CreateFactory.CreateTestFactory();

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object, _mapper);

            var addResult = await profileService.AddProfileAsync(profile);

            addResult.StatusCode.Should().Be(200);
            addResult.Value.Should().NotBeEmpty();
            addResult.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task AddProfile_ShouldReturn409_ThisProfileAlreadyExist()
        {
            _repository
                .Setup(repo => repo.AddAsync(It.IsAny<Profiles.Domain.Profile>()))
                .ReturnsAsync(Result<Guid>.Failure(409, "Account already exis"));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object, _mapper);

            Profiles.Domain.Profile profile = CreateFactory.CreateTestFactory();

            var addResult = await profileService.AddProfileAsync(profile);

            addResult.StatusCode.Should().Be(409);
        }

        [Fact]
        public async Task DeleteProfile_ShouldReturnCorrectResponse()
        {
            Guid profileId = Guid.NewGuid();

            _repository
                .Setup(repo => repo.DeleteAsync(profileId))
                .ReturnsAsync(Result<bool>.Success(true));

            _repository
                .Setup(repo => repo.GetByAccountIdAsync(profileId))
                .ReturnsAsync(Result<Profiles.Domain.Profile>.Success(new Profiles.Domain.Profile()));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object, _mapper);

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

            _repository
                .Setup(repo => repo.GetByAccountIdAsync(profileId))
                .ReturnsAsync(Result<Profiles.Domain.Profile>.Failure(404, "Profile not found"));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object, _mapper);

            var deleteResult = await profileService.DeleteProfileAsync(profileId);

            deleteResult.StatusCode.Should().Be(404);
            deleteResult.Value.Should().BeFalse();
        }

        [Fact]
        public async Task GetProfileByAccountId_ShouldReturnCorrectResponse()
        {
            Profiles.Domain.Profile profile = CreateFactory.CreateTestFactory();

            _repository
                .Setup(repo => repo.GetByAccountIdAsync(profile.AccountID))
                .ReturnsAsync(Result<Profiles.Domain.Profile>.Success(profile));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object, _mapper);

            var getResult = await profileService.GetProfileByAccountIdAsync(profile.AccountID);

            getResult.StatusCode.Should().Be(200);
            getResult.Value.Should().NotBeNull();
            getResult.Value.AccountID.Should().Be(profile.AccountID);
        }

        [Fact]
        public async Task GetProfileByAccountId_ShouldReturn404_ProfileNotFound()
        {
            Profiles.Domain.Profile profile = CreateFactory.CreateTestFactory();

            _repository
                .Setup(repo => repo.GetByAccountIdAsync(profile.AccountID))
                .ReturnsAsync(Result<Profiles.Domain.Profile>.Failure(404, "Profile not found"));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object, _mapper);

            var getResult = await profileService.GetProfileByAccountIdAsync(profile.AccountID);

            getResult.StatusCode.Should().Be(404);
            getResult.Value.Should().BeNull();
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnCorrectResponse()
        {
            Profiles.Domain.Profile profile = CreateFactory.CreateTestFactory();
            profile.DateOfBirth = new DateTime(2000, 5, 5);

            _repository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Profiles.Domain.Profile>()))
                .ReturnsAsync(Result<bool>.Success(true));

            _repository
                .Setup(repo => repo.GetByAccountIdAsync(profile.AccountID))
                .ReturnsAsync(Result<Profiles.Domain.Profile>.Success(new Profiles.Domain.Profile()));

            Profiles.Domain.Profile updatedProfile = CreateFactory.CreateTestFactory();
            updatedProfile.AccountID = profile.AccountID;
            updatedProfile.DateOfBirth = profile.DateOfBirth;

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object, _mapper);

            var updateResult = await profileService.UpdateProfileAsync(profile);

            updateResult.StatusCode.Should().Be(200);
            updateResult.Value.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturn404_ProfileNotFound()
        {
            Profiles.Domain.Profile profile = CreateFactory.CreateTestFactory();

            _repository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Profiles.Domain.Profile>()))
                .ReturnsAsync(Result<bool>.Failure(404, "Profile not found"));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object, _mapper);

            var updateResult = await profileService.UpdateProfileAsync(profile);

            updateResult.StatusCode.Should().Be(404);
            updateResult.Value.Should().BeFalse();
        }

        [Fact]
        public async Task GetProfilesByFilterAsync_ShouldReturnCorrectResponse()
        {
            _repository
                .Setup(repo => repo.GetProfilesByFilterAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<int>()))
                .ReturnsAsync(Result<IEnumerable<Profiles.Domain.Profile>>.Success(new List<Profiles.Domain.Profile>
                {
                    CreateFactory.CreateTestFactory(),
                    CreateFactory.CreateTestFactory(),
                    CreateFactory.CreateTestFactory()
                }));

            var profileService = new ProfileService(_repository.Object, _mockLogger.Object, _mapper);

            var result = await profileService.GetProfilesByFilterAsync(new List<Guid>(), 3);

            result.StatusCode.Should().Be(200);
            result.Value.Should().NotBeNull();
            result.Value.Count().Should().Be(3);
        }

        [Fact]
        public async Task GetProfilesByFilterAsync_ShouldReturn400_NullGuids()
        {
            var profileService = new ProfileService(_repository.Object, _mockLogger.Object, _mapper);

            var result = await profileService.GetProfilesByFilterAsync(null, 3);

            result.StatusCode.Should().Be(400);
        }
    }
}
