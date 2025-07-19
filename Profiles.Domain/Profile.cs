using Profiles.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiles.Domain
{
    public class Profile
    {
        public Guid ProfileID { get; set; }
        public Guid AccountID { get; set; }
        public string[] ImagePaths { get; set; } = new string[3];
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
    }
}
