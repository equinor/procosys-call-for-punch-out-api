﻿using System.Collections.Generic;
using Fusion.Integration.Meeting;

namespace Equinor.ProCoSys.IPO.Query.GetInvitationById
{
    public class FunctionalRoleDto
    {
        public FunctionalRoleDto(
            string code,
            string email,
            IEnumerable<PersonDto> persons)
        {
            Code = code;
            Email = email;
            Persons = persons;
        }

        public int Id { get; set; }
        public string Code { get; }
        public string Email { get; }
        public OutlookResponse? Response { get; set; }
        public IEnumerable<PersonDto> Persons { get; }
    }
}
