using FluentValidation;
using Profiles.Application.DTOs;
using Profiles.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiles.Application.Validators
{
    public class ProfileRequestDTOValidator : AbstractValidator<ProfileRequestDTO>
    {
        public ProfileRequestDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(30).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.AccountID)
                .NotEmpty().WithMessage("Account ID is required.")
                .Must(id => id != Guid.Empty).WithMessage("Account ID cannot be an empty GUID.");

            RuleFor(x => x.Gender)
                .IsInEnum();

            RuleFor(x => x)
                .Must(HaveAtLeastThreeInterests).WithMessage("You must select at least three interests from the available options.");
        }

        private bool HaveAtLeastThreeInterests(ProfileRequestDTO profile)
        {
            int selectedInterests = 0;

            if (profile.BookInterest != null) selectedInterests++;
            if (profile.SportInterest != null) selectedInterests++;
            if (profile.MovieInterest != null) selectedInterests++;
            if (profile.MusicInterest != null) selectedInterests++;
            if (profile.FoodInterest != null) selectedInterests++;
            if (profile.LifestyleInterest != null) selectedInterests++;
            if (profile.TravelInterest != null) selectedInterests++;
            if (profile.HobbyInterest != null) selectedInterests++;

            return selectedInterests >= 3;
        }
    }
}
