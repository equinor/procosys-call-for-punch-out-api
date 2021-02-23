﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.Command.InvitationCommands.CancelPunchOut;
using Equinor.ProCoSys.IPO.Domain;
using Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate;
using Equinor.ProCoSys.IPO.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.IPO.Domain.Events.PostSave;
using Equinor.ProCoSys.IPO.ForeignApi.MainApi.McPkg;
using Fusion.Integration.Meeting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.IPO.Command.Tests.InvitationCommands.CancelPunchOut
{
    [TestClass]
    public class CancelPunchOutCommandHandlerTests
    {
        private Mock<IPlantProvider> _plantProviderMock;
        private Mock<IInvitationRepository> _invitationRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IPersonRepository> _personRepositoryMock;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;
        private Mock<IMcPkgApiService> _mcPkgApiServiceMock;
        private Mock<IFusionMeetingClient> _meetingClientMock;

        private CancelPunchOutCommand _command;
        private CancelPunchOutCommandHandler _dut;
        private const string _plant = "PCS$TEST_PLANT";
        private const string _projectName = "Project name";
        private const string _title = "Test title";
        private const string _description = "Test description";
        private const DisciplineType _type = DisciplineType.DP;
        private readonly Guid _meetingId = new Guid("11111111-2222-2222-2222-333333333333");
        private static Guid _azureOidForCurrentUser = new Guid("11111111-1111-2222-3333-333333333334");
        private const string _invitationRowVersion = "AAAAAAAAABA=";
        private Invitation _invitation;

        [TestInitialize]
        public void Setup()
        {
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock
                .Setup(x => x.Plant)
                .Returns(_plant);

            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _currentUserProviderMock
                .Setup(x => x.GetCurrentUserOid()).Returns(_azureOidForCurrentUser);

            var currentPerson = new Person(_azureOidForCurrentUser, null, null, null, null);
            _personRepositoryMock = new Mock<IPersonRepository>();
            _personRepositoryMock
                .Setup(x => x.GetByOidAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(currentPerson));

            _meetingClientMock = new Mock<IFusionMeetingClient>();

            //create invitation
            _invitation = new Invitation(
                    _plant,
                    _projectName,
                    _title,
                    _description,
                    _type,
                    new DateTime(),
                    new DateTime(),
                    null)
                { MeetingId = _meetingId };

            var participant = new Participant(_plant,
                Organization.Contractor,
                IpoParticipantType.FunctionalRole,
                "FR",
                null,
                null,
                null,
                null,
                null,
                0);

            _invitation.CompleteIpo(participant, "AAAAAAAAABB=", currentPerson, new DateTime());

            _invitationRepositoryMock = new Mock<IInvitationRepository>();
            _invitationRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(_invitation));

            _mcPkgApiServiceMock = new Mock<IMcPkgApiService>();

            //command
            _command = new CancelPunchOutCommand(_invitation.Id, _invitationRowVersion);

            _dut = new CancelPunchOutCommandHandler(
                _invitationRepositoryMock.Object,
                _personRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _currentUserProviderMock.Object);
        }

        [TestMethod]
        public async Task CancelPunchOutCommand_ShouldCancelPunchOut()
        {
            Assert.AreEqual(IpoStatus.Completed, _invitation.Status);

            await _dut.Handle(_command, default);

            Assert.AreEqual(IpoStatus.Canceled, _invitation.Status);
            _unitOfWorkMock.Verify(t => t.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task HandlingCancelIpoCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_invitationRowVersion, result.Data);
            Assert.AreEqual(_invitationRowVersion, _invitation.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingCancelIpoCommand_ShouldAddAIpoCanceledPostSaveEvent()
        {
            // Assert
            Assert.AreEqual(1, _invitation.PostSaveDomainEvents.Count);

            // Act
            await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(2, _invitation.PostSaveDomainEvents.Count);
            Assert.AreEqual(typeof(IpoCanceledEvent), _invitation.PostSaveDomainEvents.Last().GetType());
        }
    }
}
