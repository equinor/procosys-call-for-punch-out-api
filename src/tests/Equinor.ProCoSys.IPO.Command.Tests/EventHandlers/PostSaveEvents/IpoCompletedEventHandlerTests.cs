﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.Command.EventHandlers.PostSaveEvents;
using Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate;
using Equinor.ProCoSys.IPO.Domain.Events.PostSave;
using Equinor.ProCoSys.IPO.Email;
using Equinor.ProCoSys.PcsServiceBus.Sender;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.IPO.Command.Tests.EventHandlers.PostSaveEvents
{
    [TestClass]
    public class IpoCompletedEventHandlerTests
    {
        private IpoCompletedEventHandler _dut;
        private Mock<ITopicClient> _topicClient;
        private PcsBusSender _pcsBusSender;
        private Mock<IOptionsMonitor<MeetingOptions>> _meetingOptionsMock;
        private Mock<IEmailService> _emailServiceMock;

        [TestInitialize]
        public void Setup()
        {
            _topicClient = new Mock<ITopicClient>();
            _pcsBusSender = new PcsBusSender();
            _pcsBusSender.Add("ipo", _topicClient.Object);

            _meetingOptionsMock = new Mock<IOptionsMonitor<MeetingOptions>>();
            _meetingOptionsMock.Setup(m => m.CurrentValue).Returns(new MeetingOptions() { PcsBaseUrl = "baseUrl"});
            _emailServiceMock = new Mock<IEmailService>();

            _dut = new IpoCompletedEventHandler(_pcsBusSender, _emailServiceMock.Object, _meetingOptionsMock.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldSendBusTopic()
        {
            // Arrange
            var objectGuid = Guid.NewGuid();
            var plant = "TestPlant";
            var emails = new List<string> {"email1@test.com", "email2@test.com"};
            var ipoCompletedEvent = new IpoCompletedEvent(plant, objectGuid, 1234, "Invitation title", emails);

            // Act
            await _dut.Handle(ipoCompletedEvent, default);

            // Assert
            _topicClient.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Once());
        }

        [TestMethod]
        public async Task Handle_ShouldSendEmail()
        {
            // Arrange
            var objectGuid = Guid.NewGuid();
            var plant = "TestPlant";
            var commpkgs = new List<CommPkg>
            {
                new CommPkg(plant, "project", "commpkgno", "description", "status", "system|subsystem")
            };
            var invitation = new Invitation(plant, "project", "title", "description", DisciplineType.MDP, DateTime.Now,
                DateTime.Now, "location", null, commpkgs);
            invitation.AddParticipant(new Participant(plant, Organization.ConstructionCompany,
                IpoParticipantType.Person, "code", "firstname", "lastname", "username", "email", Guid.NewGuid(), 1));
            var emails = new List<string>() {"email1@test.com", "email2@test.com"};
            var ipoCompletedEvent = new IpoCompletedEvent(plant, objectGuid, invitation.Id, invitation.Title, emails);

            // Act
            await _dut.Handle(ipoCompletedEvent, default);

            // Assert
            _emailServiceMock.Verify(
                t => t.SendEmailsAsync(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}
