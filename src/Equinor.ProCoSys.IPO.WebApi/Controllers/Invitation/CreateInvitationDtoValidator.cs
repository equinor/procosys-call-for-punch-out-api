﻿using FluentValidation;

namespace Equinor.ProCoSys.IPO.WebApi.Controllers.Invitation
{
    public class CreateInvitationDtoValidator : AbstractValidator<CreateInvitationDto>
    {
        public CreateInvitationDtoValidator() => RuleFor(x => x.Meeting).NotNull().SetValidator(new CreateMeetingDtoValidator());
    }

    public class CreateMeetingDtoValidator : AbstractValidator<CreateMeetingDto>
    {
        public CreateMeetingDtoValidator()
        {
            RuleFor(x => x.BodyHtml).MaximumLength(8192);
            RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
            RuleFor(x => x.Location).MaximumLength(1024);
            RuleFor(x => x.Title).MaximumLength(1024);
        }
    }
}