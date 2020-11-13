﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.Command.InvitationCommands.ChangeAttendedStatus;
using Equinor.ProCoSys.IPO.Command.InvitationCommands.ChangeAttendedStatuses;
using Equinor.ProCoSys.IPO.Domain;
using Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate;
using Equinor.ProCoSys.IPO.ForeignApi.MainApi.Person;
using Equinor.ProCoSys.IPO.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.IPO.Command.Tests.InvitationCommands.ChangeAttendedStatuses
{
    [TestClass]
    public class ChangeAttendedStatusesCommandHandlerTests
    {
        private Mock<IPlantProvider> _plantProviderMock;
        private Mock<IInvitationRepository> _invitationRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IPersonApiService> _personApiServiceMock;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;

        private ChangeAttendedStatusesCommand _command;
        private ChangeAttendedStatusesCommandHandler _dut;
        private const string _plant = "PCS$TEST_PLANT";
        private const string _projectName = "Project name";
        private const string _title = "Test title";
        private const string _description = "Test description";
        private const DisciplineType _type = DisciplineType.DP;
        private readonly Guid _meetingId = new Guid("11111111-2222-2222-2222-333333333333");
        private const string _invitationRowVersion = "AAAAAAAAABA=";
        private const string _participantRowVersion1 = "AAAAAAAAABB=";
        private const string _participantRowVersion2 = "AAAAAAAAABM=";
        private int _saveChangesCount;
        private static Guid _azureOidForCurrentUser = new Guid("12345678-1234-1234-1234-123456789123");
        private const string _functionalRoleCode = "FR1";
        private const int _contractorParticipantId = 20;
        private const int _constructionCompanyParticipantId = 30;
        private Invitation _invitation;

        private readonly List<ParticipantToChangeAttendedStatusForCommand> _participants = new List<ParticipantToChangeAttendedStatusForCommand>
        {
            new ParticipantToChangeAttendedStatusForCommand(
                _contractorParticipantId,
                true,
                _participantRowVersion1),
            new ParticipantToChangeAttendedStatusForCommand(
                _constructionCompanyParticipantId,
                false,
                _participantRowVersion2)
        };

        [TestInitialize]
        public void Setup()
        {
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock
                .Setup(x => x.Plant)
                .Returns(_plant);

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => _saveChangesCount++);

            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _currentUserProviderMock
                .Setup(x => x.GetCurrentUserOid()).Returns(_azureOidForCurrentUser);


            //mock person response from main API
            var personDetails = new ProCoSysPerson
            {
                AzureOid = _azureOidForCurrentUser.ToString(),
                FirstName = "Ola",
                LastName = "Nordman",
                Email = "ola@test.com",
                UserName = "ON"
            };

            _personApiServiceMock = new Mock<IPersonApiService>();
            _personApiServiceMock
                .Setup(x => x.GetPersonInFunctionalRoleAsync(_plant,
                    _azureOidForCurrentUser.ToString(), _functionalRoleCode))
                .Returns(Task.FromResult(personDetails));

            //create invitation
            _invitation = new Invitation(_plant, _projectName, _title, _description, _type) { MeetingId = _meetingId };

            var participant1 = new Participant(
                _plant,
                Organization.Contractor,
                IpoParticipantType.FunctionalRole,
                _functionalRoleCode,
                null,
                null,
                null,
                null,
                null,
                0);
            participant1.SetProtectedIdForTesting(_contractorParticipantId);
            _invitation.AddParticipant(participant1);
            var participant2 = new Participant(
                _plant,
                Organization.ConstructionCompany,
                IpoParticipantType.Person,
                null,
                "Kari",
                "Nordmann",
                "KN",
                "kari@test.com",
                null,
                1);
            participant2.SetProtectedIdForTesting(_constructionCompanyParticipantId);
            _invitation.AddParticipant(participant2);

            _invitationRepositoryMock = new Mock<IInvitationRepository>();
            _invitationRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(_invitation));
            _invitation.Status = IpoStatus.Completed;

            //command
            _command = new ChangeAttendedStatusesCommand(
                _invitation.Id,
                _invitationRowVersion,
                _participants);

            _dut = new ChangeAttendedStatusesCommandHandler(
                _plantProviderMock.Object,
                _invitationRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _currentUserProviderMock.Object,
                _personApiServiceMock.Object);
        }

        [TestMethod]
        public async Task ChangeAttendedStatusesCommand_ShouldChangeStatuses()
        {
            //Assert.AreEqual(IpoStatus.Completed, _invitation.Status);
            Assert.AreEqual(false, _invitation.Participants.First().Attended);

            await _dut.Handle(_command, default);

            Assert.AreEqual(true, _invitation.Participants.First().Attended);
        }

        [TestMethod]
        public async Task HandlingCompleteIpoCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_invitationRowVersion, _invitation.RowVersion.ConvertToString());
            Assert.AreEqual(_participantRowVersion1, _invitation.Participants.ToList()[0].RowVersion.ConvertToString());
        }
    }
}
