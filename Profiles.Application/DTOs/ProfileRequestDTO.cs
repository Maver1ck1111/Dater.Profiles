using Microsoft.AspNetCore.Http;
using Profiles.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiles.Application.DTOs
{
    public class ProfileRequestDTO
    {
        public Guid AccountID { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public BookInterest? BookInterest { get; set; }
        public SportInterest? SportInterest { get; set; }
        public MovieInterest? MovieInterest { get; set; }
        public MusicInterest? MusicInterest { get; set; }
        public FoodInterest? FoodInterest { get; set; }
        public LifestyleInterest? LifestyleInterest { get; set; }
        public TravelInterest? TravelInterest { get; set; }
        public HobbyInterest? HobbyInterest { get; set; }
    }
}
