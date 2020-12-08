﻿using System;

namespace Equinor.ProCoSys.IPO.WebApi.IntegrationTests.Invitations
{
    public class PersonDto
    { 
        public int? Id { get; set; }
        public string RowVersion { get; set; }
        public Guid? AzureOid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}