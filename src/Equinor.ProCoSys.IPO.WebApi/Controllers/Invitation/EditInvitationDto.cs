﻿using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.IPO.WebApi.Controllers.Invitation
{
    public class EditInvitationDto
    {
        public EditMeetingDto Meeting { get; set; }
    }

    public class EditMeetingDto
    {
        public string Title { get; set; }
        public string BodyHtml { get; set; }
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public IEnumerable<Guid> ParticipantOids { get; set; }
    }
}