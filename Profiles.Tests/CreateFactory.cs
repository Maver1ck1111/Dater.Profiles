using Profiles.Domain.Enums;
using Profiles.Domain;
using Profiles.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Profiles.Tests
{
    public class CreateFactory
    {
        public static Profile CreateTestFactory() => new Profile()
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

        public static ProfileRequestDTO CreateDTOTestFactory() => new ProfileRequestDTO()
        {
            Name = "Test User",
            Description = "Test Description",
            DateOfBirth = new DateTime(1990, 1, 1),
            AccountID = Guid.NewGuid(),
            BookInterest = BookInterest.Fantasy,
            MovieInterest = MovieInterest.Thriller,
            MusicInterest = MusicInterest.Rock
        };

        private static IFormFile CreateTestFormFile(string fileName, string contentType, string content)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return new FormFile(stream, 0, stream.Length, "Data", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
    }
}
