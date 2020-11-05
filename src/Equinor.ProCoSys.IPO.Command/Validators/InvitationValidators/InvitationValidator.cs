﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.Command.InvitationCommands;
using Equinor.ProCoSys.IPO.Domain;
using Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.IPO.Command.Validators.InvitationValidators
{
    public class InvitationValidator : IInvitationValidator
    {
        private readonly IReadOnlyContext _context;

        public InvitationValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> IpoExistsAsync(int invitationId, CancellationToken token) =>
            await (from ipo in _context.QuerySet<Invitation>()
                where ipo.Id == invitationId
                select ipo).AnyAsync(token);

        public bool IsValidScope(
            IList<string> mcPkgScope,
            IList<string> commPkgScope) 
                => (mcPkgScope.Count > 0 || commPkgScope.Count > 0) && (mcPkgScope.Count < 1 || commPkgScope.Count < 1);

        public async Task<bool> IpoTitleExistsInProjectAsync(string projectName, string title, CancellationToken token)
        => await(from invitation in _context.QuerySet<Invitation>()
                where invitation.Title == title && invitation.ProjectName == projectName
                select invitation).AnyAsync(token);

        public async Task<bool> IpoTitleExistsInProjectOnAnotherIpoAsync(string projectName, string title, int id, CancellationToken token)
            => await (from invitation in _context.QuerySet<Invitation>()
                where invitation.Title == title && 
                      invitation.ProjectName == projectName &&
                      invitation.Id != id
                select invitation).AnyAsync(token);

        private bool IsValidExternalParticipant(ParticipantsForCommand participant)
        { 
            var isValidEmail = new EmailAddressAttribute().IsValid(participant.ExternalEmail.Email);
            return isValidEmail && participant.Person == null && participant.FunctionalRole == null;
        }

        private bool IsValidPerson(ParticipantsForCommand participant)
        {
            if (participant.Person.Email == null && (participant.Person.AzureOid == Guid.Empty || participant.Person.AzureOid == null))
            {
                return false;
            }
            var isValidEmail = new EmailAddressAttribute().IsValid(participant.Person.Email);
            return participant.ExternalEmail == null && participant.FunctionalRole == null && isValidEmail;
        }

        private bool IsValidFunctionalRole(ParticipantsForCommand participant)
        {
            foreach (var person in participant.FunctionalRole.Persons)
            {
                if (!(new EmailAddressAttribute().IsValid(person.Email)))
                {
                    return false;
                }
            }
            
            return participant.Person == null && participant.ExternalEmail == null;
        }

        public bool IsValidParticipantList(IList<ParticipantsForCommand> participants)
        {
            foreach (var p in participants)
            {
                if (p.ExternalEmail == null && p.Person == null && p.FunctionalRole == null)
                {
                    return false;
                }
                if (p.Organization == Organization.External && !IsValidExternalParticipant(p))
                {
                    return false;
                }
                if (p.Person != null && !IsValidPerson(p))
                {
                    return false;
                }
                if (p.FunctionalRole != null && !IsValidFunctionalRole(p))
                {
                    return false;
                }
            }

            return true;
        }

        public bool RequiredParticipantsMustBeInvited(IList<ParticipantsForCommand> participants)
        {
            if (participants.Count < 2)
            {
                return false;
            }

            return participants[0].Organization == Organization.Contractor &&
                   participants[0].ExternalEmail == null &&
                   participants[1].Organization == Organization.ConstructionCompany &&
                   participants[1].ExternalEmail == null;
        }

        public bool OnlyRequiredParticipantsHaveLowestSortKeys(IList<ParticipantsForCommand> participants)
        {
            if (participants.Count < 2 || participants[0].SortKey != 0 || participants[1].SortKey != 1)
            {
                return false;
            }

            for (var i = 2; i < participants.Count; i++)
            {
                if (participants[i].SortKey == 0 || participants[i].SortKey == 1)
                { 
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> ParticipantExists(int? id, CancellationToken token) 
            => await(from p in _context.QuerySet<Participant>()
                where p.Id == id
                select p).AnyAsync(token);

        public async Task<bool> ParticipantWithIdExistsAsync(ParticipantsForCommand participant, CancellationToken token)
        {
            if (participant.Person?.Id != null && !await ParticipantExists(participant.Person.Id, token))
            {
                return false;
            }
            if (participant.ExternalEmail?.Id != null && !await ParticipantExists(participant.ExternalEmail.Id, token))
            {
                return false;
            }
            if (participant.FunctionalRole != null)
            {
                if (!await ParticipantExists(participant.FunctionalRole.Id, token))
                {
                    return false;
                }

                foreach (var person in participant.FunctionalRole.Persons)
                {
                    if (person.Id != null && !await ParticipantExists(person.Id, token))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        public async Task<bool> ProjectNameIsNotChangedAsync(string projectName, int id, CancellationToken token) 
            => await (from invitation in _context.QuerySet<Invitation>() 
                where invitation.Id == id && invitation.ProjectName == projectName
                select invitation).AnyAsync(token);
    }
}
