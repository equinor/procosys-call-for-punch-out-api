﻿using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.Command.EventHandlers.PostSaveEvents;
using Equinor.ProCoSys.IPO.Domain.Events.PostSave;
using Equinor.ProCoSys.PcsServiceBus.Sender;
using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.IPO.Command.Tests.EventHandlers.PostSaveEvents
{
    [TestClass]
    public class IpoAcceptedEventHandlerTests
    {
        private IpoAcceptedEventHandler _dut;
        private Mock<ITopicClient> _topicClient;
        private PcsBusSender _pcsBusSender;

        [TestInitialize]
        public void Setup()
        {
            _topicClient = new Mock<ITopicClient>();
            _pcsBusSender = new PcsBusSender();
            _pcsBusSender.Add("ipo", _topicClient.Object);
            _dut = new IpoAcceptedEventHandler(_pcsBusSender);
        }

        [TestMethod]
        public async Task Handle_ShouldSendBusTopic()
        {
            // Arrange
            var objectGuid = Guid.NewGuid();
            var plant = "TestPlant";
            var ipoAcceptedEvent = new IpoAcceptedEvent(plant, objectGuid);

            // Act
            await _dut.Handle(ipoAcceptedEvent, default);

            // Assert
            _topicClient.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Once());
        }
    }
}
