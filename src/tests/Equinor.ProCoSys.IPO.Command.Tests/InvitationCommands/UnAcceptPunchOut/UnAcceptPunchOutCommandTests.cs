﻿using System;
using Equinor.ProCoSys.IPO.Command.InvitationCommands.UnAcceptPunchOut;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.IPO.Command.Tests.InvitationCommands.UnAcceptPunchOut
{
    [TestClass]
    public class UnAcceptPunchOutCommandTests
    {
        private const string _invitationRowVersion = "AAAAAAAAABA=";
        private const string _constructionCompanyRowVersion = "AAAAAAAAABB=";
        private static readonly Guid _objectGuid = new Guid("11111111-1111-2222-3333-333333333334");

        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new UnAcceptPunchOutCommand(
                1,
                _objectGuid,
                _invitationRowVersion,
                _constructionCompanyRowVersion);

            Assert.AreEqual(1, dut.InvitationId);
            Assert.AreEqual(_objectGuid, dut.ObjectGuid);
            Assert.AreEqual(_invitationRowVersion, dut.InvitationRowVersion);
            Assert.AreEqual(_constructionCompanyRowVersion, dut.ParticipantRowVersion);
        }
    }
}
