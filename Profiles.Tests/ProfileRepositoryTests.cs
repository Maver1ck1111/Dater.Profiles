using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Profiles.Domain;
using Profiles.Domain.Enums;
using Profiles.Infrastructure;
using Profiles.Infrastructure.Repositories;

namespace Profiles.Tests
{
    public class ProfileRepositoryTests
    {
        private readonly Mock<ILogger<ProfileRepository>> _loggerMock = new Mock<ILogger<ProfileRepository>>();
        private readonly ProfileRepository _repository;

        public ProfileRepositoryTests()
        {
            Environment.SetEnvironmentVariable("Host", "localhost");
            Environment.SetEnvironmentVariable("Port", "5432");
            Environment.SetEnvironmentVariable("DatabaseName", "Profiles");
            Environment.SetEnvironmentVariable("Username", "postgres");
            Environment.SetEnvironmentVariable("Password", "1234");

            _repository = new ProfileRepository(new ProfilesDbContext(), _loggerMock.Object);
        }


        [Fact]
        public async Task AddAsync_ShouldReturnCorrecteResponse()
        {
            var profile = CreateFactory.CreateTestFactory();

            var result = await _repository.AddAsync(profile);

            result.StatusCode.Should().Be(200);

            result.Value.Should().NotBeEmpty();
            result.Value.Should().NotBe(Guid.Empty);

            var addedProfile = await _repository.GetByAccountIdAsync(profile.AccountID);

            addedProfile.StatusCode.Should().Be(200);
            addedProfile.Value.Should().NotBeNull();
            addedProfile.Value.Should().BeEquivalentTo(profile, options => options.Excluding(x => x.ProfileID));
            addedProfile.Value.AccountID.Should().Be(result.Value);
        }

        [Fact]
        public async Task AddAsync_ShouldReturn409Error_ProfileAlreadyExist()
        {
            var addedProfile = CreateFactory.CreateTestFactory();
            await _repository.AddAsync(addedProfile);

            var anotherProfile = CreateFactory.CreateTestFactory();
            anotherProfile.AccountID = addedProfile.AccountID;

            var result = await _repository.AddAsync(anotherProfile);

            result.StatusCode.Should().Be(409);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectProfile()
        {
            var profile = CreateFactory.CreateTestFactory();

            await _repository.AddAsync(profile);

            var result = await _repository.GetByAccountIdAsync(profile.AccountID);

            result.StatusCode.Should().Be(200);
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(profile, options => options.Excluding(x => x.ProfileID));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn404Error_InccorectProfileID()
        {
            var result = await _repository.GetByAccountIdAsync(Guid.NewGuid());

            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnCorrectResponse()
        {
            var profile = CreateFactory.CreateTestFactory();

            await _repository.AddAsync(profile);

            var updateProfile = CreateFactory.CreateTestFactory();

            updateProfile.AccountID = profile.AccountID;
            updateProfile.DateOfBirth = new DateTime(1995, 5, 5);

            var result = await _repository.UpdateAsync(updateProfile);

            result.StatusCode.Should().Be(200);
            result.Value.Should().BeTrue();

            var updatedProfileResult = await _repository.GetByAccountIdAsync(profile.AccountID);

            updatedProfileResult.StatusCode.Should().Be(200);
            updatedProfileResult.Value.Should().NotBeNull();
            updatedProfileResult.Value.Should().BeEquivalentTo(profile, options => options.Excluding(x => x.ProfileID).Excluding(x => x.DateOfBirth));
            updatedProfileResult.Value.DateOfBirth.Should().Be(new DateTime(1995, 5, 5));
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturn404Error_IncorectProfileID()
        {
            var profile = CreateFactory.CreateTestFactory();
            profile.ProfileID = Guid.NewGuid();
            var result = await _repository.UpdateAsync(profile);

            result.StatusCode.Should().Be(404);
            result.Value.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnCorrectResponse()
        {
            var profile = CreateFactory.CreateTestFactory();

            await _repository.AddAsync(profile);

            var result = await _repository.DeleteAsync(profile.AccountID);

            result.StatusCode.Should().Be(200);
            result.Value.Should().BeTrue();

            var deletedProfile = await _repository.GetByAccountIdAsync(profile.AccountID);

            deletedProfile.StatusCode.Should().Be(404);
        }
    }
}
