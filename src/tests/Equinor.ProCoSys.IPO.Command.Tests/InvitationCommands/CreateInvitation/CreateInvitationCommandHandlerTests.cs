﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.Command.InvitationCommands.CreateInvitation;
using Equinor.ProCoSys.IPO.Domain;
using Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate;
using Fusion.Integration.Meeting;
using Fusion.Integration.Meeting.Http.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.IPO.Command.Tests.InvitationCommands.CreateInvitation
{
    [TestClass]
    public class CreateInvitationCommandHandlerTests
    {
        private Mock<IPlantProvider> _plantProviderMock;
        private Mock<IFusionMeetingClient> _meetingClientMock;
        private Mock<IInvitationRepository> _invitationRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;

        private readonly string _plant = "PCS$TEST_PLANT";
        private Guid _meetingId = new Guid("11111111-2222-2222-2222-333333333333");
        private List<Guid> _participantIds = new List<Guid>() { new Guid("22222222-3333-3333-3333-444444444444") };

        private Invitation _createdInvitation;
        private int _saveChangesCount;

        [TestInitialize]
        public void Setup()
        {
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock
                .Setup(x => x.Plant)
                .Returns(_plant);

            _meetingClientMock = new Mock<IFusionMeetingClient>();
            _meetingClientMock
                .Setup(x => x.CreateMeetingAsync(It.IsAny<Action<GeneralMeetingBuilder>>()))
                .Returns(Task.FromResult(
                new GeneralMeeting(
                new ApiGeneralMeeting()
                {
                    Classification = string.Empty,
                    Contract = null,
                    Convention = string.Empty,
                    DateCreatedUtc = DateTime.MinValue,
                    DateEnd = new ApiDateTimeTimeZoneModel(),
                    DateStart = new ApiDateTimeTimeZoneModel(),
                    ExternalId = null,
                    Id = _meetingId,
                    InviteBodyHtml = string.Empty,
                    IsDisabled = false,
                    IsOnlineMeeting = false,
                    Location = string.Empty,
                    Organizer = new ApiPersonDetailsV1(),
                    OutlookMode = string.Empty,
                    Participants = new List<ApiMeetingParticipant>(),
                    Project = null,
                    ResponsiblePersons = new List<ApiPersonDetailsV1>(),
                    Series = null,
                    Title = string.Empty
                })));

            _invitationRepositoryMock = new Mock<IInvitationRepository>();
            _invitationRepositoryMock
                .Setup(x => x.Add(It.IsAny<Invitation>()))
                .Callback<Invitation>(x => _createdInvitation = x);

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => _saveChangesCount++);
        }

        [TestMethod]
        public async Task Invitation_is_added_to_repository_test()
        {
            var dut = new CreateInvitationCommandHandler(_plantProviderMock.Object, _meetingClientMock.Object, _invitationRepositoryMock.Object, _unitOfWorkMock.Object);

            var meeting = new CreateMeetingCommand(
                    "title",
                    "body",
                    "location",
                    new DateTime(2020, 9, 1, 12, 0, 0, DateTimeKind.Utc),
                    new DateTime(2020, 9, 1, 13, 0, 0, DateTimeKind.Utc),
                    _participantIds);
            var command = new CreateInvitationCommand(meeting);

            await dut.Handle(command, default);

            Assert.IsNotNull(_createdInvitation);
            Assert.AreEqual(2, _saveChangesCount);
        }

        [TestMethod]
        public async Task Meeting_is_created_and_meeting_id_is_set_on_invitation_test()
        {
            var dut = new CreateInvitationCommandHandler(_plantProviderMock.Object, _meetingClientMock.Object, _invitationRepositoryMock.Object, _unitOfWorkMock.Object);

            var meeting = new CreateMeetingCommand(
                    "title",
                    "body",
                    "location",
                    new DateTime(2020, 9, 1, 12, 0, 0, DateTimeKind.Utc),
                    new DateTime(2020, 9, 1, 13, 0, 0, DateTimeKind.Utc),
                    _participantIds);
            var command = new CreateInvitationCommand(meeting);

            var result = await dut.Handle(command, default);

            _meetingClientMock.Verify(x => x.CreateMeetingAsync(It.IsAny<Action<GeneralMeetingBuilder>>()), Times.Once);
            Assert.AreEqual(_meetingId, _createdInvitation.MeetingId);
        }
    }
}