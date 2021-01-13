﻿using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.IPO.WebApi.IntegrationTests.Participants
{
    [TestClass]
    public class ParticipantsControllerNegativeTests : ParticipantsControllerTestsBase
    {
        #region GetFunctionalRolesForIpo

        [TestMethod]
        public async Task GetFunctionalRolesForIpo_AsAnonymous_ShouldReturnUnauthorized()
            => await ParticipantsControllerTestsHelper.GetFunctionalRolesForIpoAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetFunctionalRolesForIpo_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ParticipantsControllerTestsHelper.GetFunctionalRolesForIpoAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetFunctionalRolesForIpo_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await ParticipantsControllerTestsHelper.GetFunctionalRolesForIpoAsync(
                UserType.Hacker,
                TestFactory.PlantWithAccess,
                HttpStatusCode.Forbidden);
        #endregion

        #region GetPersons
        [TestMethod]
        public async Task GetPersons_AsAnonymous_ShouldReturnUnauthorized()
            => await ParticipantsControllerTestsHelper.GetPersonsAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                "p",
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetPersons_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ParticipantsControllerTestsHelper.GetPersonsAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                "p",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetPersons_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await ParticipantsControllerTestsHelper.GetPersonsAsync(
                UserType.Hacker,
                TestFactory.PlantWithAccess,
                "p",
                HttpStatusCode.Forbidden);
        #endregion

        #region GetRequiredSignerPersons
        [TestMethod]
        public async Task GetRequiredSignerPersons_AsAnonymous_ShouldReturnUnauthorized()
            => await ParticipantsControllerTestsHelper.GetRequiredSignerPersonsAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                "p",
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetRequiredSignerPersons_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ParticipantsControllerTestsHelper.GetRequiredSignerPersonsAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                "p",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetRequiredSignerPersons_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await ParticipantsControllerTestsHelper.GetRequiredSignerPersonsAsync(
                UserType.Hacker,
                TestFactory.PlantWithAccess,
                "p",
                HttpStatusCode.Forbidden);
        #endregion

        #region GetAdditionalSignerPersons
        [TestMethod]
        public async Task GetAdditionalSignerPersons_AsAnonymous_ShouldReturnUnauthorized()
            => await ParticipantsControllerTestsHelper.GetAdditionalSignerPersonsAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                "p",
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAdditionalSignerPersons_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ParticipantsControllerTestsHelper.GetAdditionalSignerPersonsAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                "p",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAdditionalSignerPersons_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await ParticipantsControllerTestsHelper.GetAdditionalSignerPersonsAsync(
                UserType.Hacker,
                TestFactory.PlantWithAccess,
                "p",
                HttpStatusCode.Forbidden);
        #endregion
    }
}