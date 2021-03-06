﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate;
using Equinor.ProCoSys.IPO.Infrastructure.Repositories;
using Equinor.ProCoSys.IPO.Test.Common.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.ProCoSys.IPO.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class InvitationRepositoryTests : RepositoryTestBase
    {
        private const int InvitationWithMcPkgId = 5;
        private const int InvitationWithMcPkgMoveId = 6;
        private const int McPkgId = 51;
        private const int McPkgId2 = 52;
        private const int McPkgId3 = 54;
        private const int CommPkgId = 71;
        private const int CommPkgId4 = 714;
        private const int ParticipantId = 1;
        private string _projectName = "ProjectName";
        private string _projectName2 = "ProjectName2";
        private string _mcPkgNo = "MC1";
        private string _mcPkgNo2 = "MC2";
        private string _mcPkgNo3 = "MC3";
        private string _system = "1|2";
        private string _commPkgNo = "Comm1";
        private string _commPkgNo2 = "Comm2";
        private string _commPkgNo3 = "Comm3";
        private string _commPkgNo4 = "Comm4";
        private List<Invitation> _invitations;
        private Mock<DbSet<Invitation>> _dbInvitationSetMock;
        private Mock<DbSet<Attachment>> _attachmentSetMock;
        private Mock<DbSet<Participant>> _participantSetMock;
        private Mock<DbSet<McPkg>> _mcPkgSetMock;
        private Mock<DbSet<CommPkg>> _commPkgSetMock;
        private Mock<DbSet<Comment>> _commentSetMock;

        private InvitationRepository _dut;
        private McPkg _mcPkg, _mcPkg2, _mcPkg3;
        private CommPkg _commPkg;
        private Participant _participant;
        private Attachment _attachment;
        private Comment _comment;
        private Invitation _dpInviation;
        private Invitation _mdpInvitation;
        private Invitation _mdpInvitationWithTwoCommpkgs;
        private CommPkg _commPkg2;
        private Invitation _dpInviationMove;
        private CommPkg _commPkg4;
        private Invitation _mdpInvitation4;

        [TestInitialize]
        public void Setup()
        {
            _mcPkg = new McPkg(TestPlant, _projectName2, _commPkgNo2, _mcPkgNo, "Description", _system);
            _mcPkg.SetProtectedIdForTesting(McPkgId);

            _commPkg = new CommPkg(TestPlant, _projectName, _commPkgNo, "Description", "OK", "1|2");
            _commPkg.SetProtectedIdForTesting(CommPkgId);

            _commPkg2 = new CommPkg(TestPlant, _projectName, _commPkgNo2, "Description", "OK", "1|2");
            _commPkg.SetProtectedIdForTesting(CommPkgId);
            
            _commPkg4 = new CommPkg(TestPlant, _projectName, _commPkgNo4, "Description", "OK", "1|2");
            _commPkg4.SetProtectedIdForTesting(CommPkgId4);

            _mcPkg2 = new McPkg(TestPlant, _projectName, _commPkgNo, _mcPkgNo2, "Description", _system);
            _mcPkg2.SetProtectedIdForTesting(McPkgId2);

            _mcPkg3 = new McPkg(TestPlant, _projectName, _commPkgNo3, _mcPkgNo3, "Description", _system);
            _mcPkg3.SetProtectedIdForTesting(McPkgId3);

            _participant = new Participant(
                TestPlant,
                Organization.Contractor,
                IpoParticipantType.FunctionalRole,
                "FR",
                null,
                null,
                null,
                "fr@test.com",
                null,
                0);
            _participant.SetProtectedIdForTesting(ParticipantId);

            _dpInviation = new Invitation(
                TestPlant,
                _projectName2,
                "Title",
                "D",
                DisciplineType.DP,
                new DateTime(),
                new DateTime(),
                null,
                new List<McPkg> {_mcPkg},
                null);
            _dpInviation.SetProtectedIdForTesting(InvitationWithMcPkgId);
            _dpInviationMove = new Invitation(
                TestPlant,
                _projectName,
                "Title",
                "D",
                DisciplineType.DP,
                new DateTime(),
                new DateTime(),
                null,
                new List<McPkg> { _mcPkg3 },
                null);
            _dpInviationMove.SetProtectedIdForTesting(InvitationWithMcPkgMoveId);
            _mdpInvitation = new Invitation(
                TestPlant,
                _projectName,
                "Title 2",
                "D",
                DisciplineType.MDP,
                new DateTime(),
                new DateTime(),
                null,
                null,
                new List<CommPkg> {_commPkg});
            _mdpInvitationWithTwoCommpkgs = new Invitation(
                TestPlant,
                _projectName,
                "Title 3",
                "D",
                DisciplineType.MDP,
                new DateTime(),
                new DateTime(),
                null,
                null,
                new List<CommPkg> { _commPkg, _commPkg2 });

            _attachment = new Attachment(TestPlant, "filename.txt");

            _dpInviation.AddParticipant(_participant);
            _dpInviation.AddAttachment(_attachment);

            _comment = new Comment(TestPlant, "comment");
            _mdpInvitation.AddComment(_comment);

            _mdpInvitation4 = new Invitation(
                TestPlant,
                _projectName,
                "Title 4",
                "D",
                DisciplineType.MDP,
                new DateTime(),
                new DateTime(),
                null,
                null,
                new List<CommPkg> { _commPkg4 });

            _invitations = new List<Invitation>
            {
                _dpInviation,
                _dpInviationMove,
                _mdpInvitation,
                _mdpInvitationWithTwoCommpkgs,
                _mdpInvitation4
            };

            _dbInvitationSetMock = _invitations.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Invitations)
                .Returns(_dbInvitationSetMock.Object);

            var attachments = new List<Attachment>
            {
                _attachment
            };

            _attachmentSetMock = attachments.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Attachments)
                .Returns(_attachmentSetMock.Object);

            var participants = new List<Participant>
            {
                _participant
            };

            _participantSetMock = participants.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Participants)
                .Returns(_participantSetMock.Object);

            var mcPkgs = new List<McPkg>
            {
                _mcPkg,
                _mcPkg2,
                _mcPkg3
            };

            _mcPkgSetMock = mcPkgs.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.McPkgs)
                .Returns(_mcPkgSetMock.Object);

            var commPkgs = new List<CommPkg>
            {
                _commPkg,
                _commPkg2,
                _commPkg4
            };

            _commPkgSetMock = commPkgs.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.CommPkgs)
                .Returns(_commPkgSetMock.Object);

            var comments = new List<Comment>
            {
                _comment
            };

            _commentSetMock = comments.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Comments)
                .Returns(_commentSetMock.Object);

            _dut = new InvitationRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnAllItems()
        {
            var result = await _dut.GetAllAsync();

            Assert.AreEqual(5, result.Count);
        }

        [TestMethod]
        public async Task GetByIds_UnknownId_ShouldReturnEmptyList()
        {
            var result = await _dut.GetByIdsAsync(new List<int> { 1234 });

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task Exists_KnownId_ShouldReturnTrue()
        {
            var result = await _dut.Exists(InvitationWithMcPkgId);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Exists_UnknownId_ShouldReturnFalse()
        {
            var result = await _dut.Exists(1234);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetById_KnownId_ShouldReturnInvitation()
        {
            var result = await _dut.GetByIdAsync(InvitationWithMcPkgId);

            Assert.AreEqual(InvitationWithMcPkgId, result.Id);
        }

        [TestMethod]
        public async Task GetById_UnknownId_ShouldReturnNull()
        {
            var result = await _dut.GetByIdAsync(1234);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Add_Invitation_ShouldCallAddForInvitation()
        {
            _dut.Add(_dpInviation);

            _dbInvitationSetMock.Verify(x => x.Add(_dpInviation), Times.Once);
        }

        [TestMethod]
        public void RemoveParticipant_KnownParticipant_ShouldRemoveParticipant()
        {
            _dut.RemoveParticipant(_participant);

            _participantSetMock.Verify(s => s.Remove(_participant), Times.Once);
        }

        [TestMethod]
        public void RemoveAttachment_KnownAttachment_ShouldRemoveAttachment()
        {
            _dut.RemoveAttachment(_attachment);

            _attachmentSetMock.Verify(s => s.Remove(_attachment), Times.Once);
        }

        [TestMethod]
        public void MoveCommPkg_ShouldChangeProjectRelationAndUpdateInvitation()
        {
            // Arrange & Assert
            const string toProjectName = "ProjectNameUpdated";
            const string description = "New description";
            Assert.AreNotEqual(toProjectName, _commPkg.ProjectName);
            Assert.AreNotEqual(toProjectName, _mcPkg2.ProjectName);

            // Act
            _dut.MoveCommPkg(_projectName, toProjectName, _commPkgNo3, description);

            // Assert
            Assert.AreEqual(toProjectName, _mcPkg3.ProjectName, "McPkg should be affected by comm pkg move");
            Assert.AreEqual(_projectName2, _mcPkg.ProjectName, "Only data on specific comm pkg should be updated");
            Assert.AreEqual(toProjectName, _dpInviationMove.ProjectName, "Project ref on invitation not changed when comm pkg was moved to other project");
            Assert.AreNotEqual(toProjectName, _dpInviation.ProjectName, "Project ref on invitation not changed when comm pkg was moved to other project");
        }

        [TestMethod]
        public void MoveCommPkg_ShouldChangeProjectRelationAndUpdateInvitationAndUpdateDescription()
        {
            // Arrange & Assert
            const string toProjectName = "ProjectNameUpdated";
            const string description = "New description";
            Assert.AreNotEqual(toProjectName, _commPkg.ProjectName);
            Assert.AreNotEqual(toProjectName, _mcPkg2.ProjectName);

            // Act
            _dut.MoveCommPkg(_projectName, toProjectName, _commPkgNo4, description);

            // Assert
            Assert.AreNotEqual(toProjectName, _mcPkg3.ProjectName, "McPkg should not be affected by comm pkg move");
            Assert.AreEqual(_projectName2, _mcPkg.ProjectName, "Only data on specific comm pkg should be updated");
            Assert.AreEqual(toProjectName, _mdpInvitation4.ProjectName, "Project ref on invitation not changed when comm pkg was moved to other project");
            Assert.AreEqual(toProjectName, _commPkg4.ProjectName);
            Assert.AreEqual(description, _commPkg4.Description);
            Assert.AreNotEqual(toProjectName, _dpInviation.ProjectName, "Project ref on invitation not changed when comm pkg was moved to other project");
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MoveCommPkg_ShouldFailWhenMultipleCommPkgsInAffectedInvitations()
        {
            // Arrange
            const string toProjectName = "ProjectNameUpdated";
            const string description = "New description";

            // Act
            _dut.MoveCommPkg(_projectName, toProjectName, _commPkgNo, description);
        }

        [TestMethod]
        public void MoveCommPkg_WithMcPkg_ShouldChangeProjectRelations()
        {
            // Arrange
            const string toProjectName = "ProjectNameUpdated";
            const string description = "New description";

            // Act
            _dut.MoveCommPkg(_projectName2, toProjectName, _commPkgNo2, description);

            // Assert
            Assert.AreEqual(toProjectName, _dpInviation.ProjectName, "Project name on invitation should be updated when comm pkg changes project");
            Assert.AreNotEqual(description, _commPkg.Description, "Only correct comm pkg should update values");
            Assert.AreEqual(toProjectName, _mcPkg.ProjectName, "Mc pkg project name must update when comm changes project");
        }

        [TestMethod]
        public void MoveCommPkg_WhenInvitationContainMultipleCommPkgs_ShouldFailDueToInvalidProjectReference()
        {
            // Arrange & Assert
            const string toProjectName = "ProjectNameUpdated";
            const string description = "New description";

            // Act
            _dut.MoveCommPkg(_projectName2, toProjectName, _commPkgNo2, description);

            // Assert
            Assert.AreEqual(toProjectName, _dpInviation.ProjectName, "Project name on invitation should be updated when comm pkg changes project");
            Assert.AreNotEqual(description, _commPkg.Description, "Only correct comm pkg should update values");
            Assert.AreEqual(toProjectName, _mcPkg.ProjectName, "Mc pkg project name must update when comm changes project");
        }

        [TestMethod]
        public void MoveMcPkg_AsRename_ShouldUpdateMcPkgNo()
        {
            // Arrange & Assert
            const string toMcPkgNo = "McPkgNo2";
            const string description = "New description";
            Assert.AreNotEqual(toMcPkgNo, _mcPkg.McPkgNo);

            // Act
            _dut.MoveMcPkg(_projectName2, _commPkgNo2, _commPkgNo2, _mcPkgNo, toMcPkgNo, description);

            // Assert
            Assert.AreEqual(toMcPkgNo, _mcPkg.McPkgNo);
            Assert.AreEqual(description, _mcPkg.Description);
        }

        [TestMethod]
        public void MoveMcPkg_WithoutRename_ShouldUpdateCommPkgNo()
        {
            // Arrange & Assert
            const string toCommPkgNo = "McPkgNo2";
            const string description = "New description";
            Assert.AreNotEqual(toCommPkgNo, _mcPkg.CommPkgNo);

            // Act
            _dut.MoveMcPkg(_projectName2, _commPkgNo2, toCommPkgNo, _mcPkgNo, _mcPkgNo, description);

            // Assert
            Assert.AreEqual(toCommPkgNo, _mcPkg.CommPkgNo);
            Assert.AreEqual(description, _mcPkg.Description);
        }

        [TestMethod]
        public void MoveMcPkg_WithRename_ShouldUpdateCommPkgNoAndMcPkgNo()
        {
            // Arrange & Assert
            const string toMcPkgNo = "McPkgNo2";
            const string toCommPkgNo = "McPkgNo2";
            const string description = "New description";
            Assert.AreNotEqual(toMcPkgNo, _mcPkg.McPkgNo);
            Assert.AreNotEqual(toCommPkgNo, _mcPkg.CommPkgNo);

            // Act
            _dut.MoveMcPkg(_projectName2, _commPkgNo2, toCommPkgNo, _mcPkgNo, toMcPkgNo, description);

            // Assert
            Assert.AreEqual(toMcPkgNo, _mcPkg.McPkgNo);
            Assert.AreEqual(toCommPkgNo, _mcPkg.CommPkgNo);
            Assert.AreEqual(description, _mcPkg.Description);
        }

        [TestMethod]
        public void UpdateCommPkg_ShouldUpdateCommPkgWithGiveNo()
        {
            // Arrange & Assert
            var newDescription = "What an amazing description!";
            Assert.AreNotEqual(newDescription, _commPkg.Description);

            // Act
            _dut.UpdateCommPkgOnInvitations(_commPkg.ProjectName, _commPkg.CommPkgNo, newDescription);

            // Assert
            Assert.AreEqual(newDescription, _commPkg.Description);

        }

        [TestMethod]
        public void UpdateMcPkg_ShouldUpdateMcPkgWithGiveNo()
        {
            // Arrange & Assert
            var newDescription = "What an amazing description!";
            Assert.AreNotEqual(newDescription, _mcPkg.Description);

            // Act
            _dut.UpdateMcPkgOnInvitations(_mcPkg.ProjectName, _mcPkg.McPkgNo, newDescription);

            // Assert
            Assert.AreEqual(newDescription, _mcPkg.Description);
        }
    }
}
