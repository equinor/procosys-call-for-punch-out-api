﻿using System.Collections.Generic;
using Equinor.ProCoSys.IPO.ForeignApi.LibraryApi.FunctionalRole;

namespace Equinor.ProCoSys.IPO.WebApi.IntegrationTests.Participants
{
    public class ProCoSysFunctionalRoleDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string InformationEmail { get; set; }
        public bool? UsePersonalEmail { get; set; }
        public IEnumerable<Person> Persons { get; set; }
    }
}
