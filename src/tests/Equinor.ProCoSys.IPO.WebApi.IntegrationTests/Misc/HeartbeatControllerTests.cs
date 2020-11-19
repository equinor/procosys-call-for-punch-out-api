﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Equinor.ProCoSys.IPO.WebApi.IntegrationTests.Misc
{
    [TestClass]
    public class HeartbeatControllerTests : TestBase
    {
        private const string _route = "Heartbeat";

        [TestMethod]
        public async Task Get_IsAlive_AsAnonymous_ShouldReturnOk() => await AssertIsAlive(AnonymousClient(null));

        [TestMethod]
        public async Task Get_IsAlive_AsHacker_ShouldReturnOk() => await AssertIsAlive(AuthenticatedHackerClient(null));

        private static async Task AssertIsAlive(HttpClient client)
        {
            var response = await client.GetAsync($"{_route}/IsAlive");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
            var dto = JsonConvert.DeserializeObject<HeartbeatDto>(content);
            Assert.IsNotNull(dto);
            Assert.IsTrue(dto.IsAlive);
            Assert.IsNotNull(dto.TimeStamp);
        }
    }
}
