﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.Domain;
using Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate;
using Equinor.ProCoSys.IPO.ForeignApi.MainApi.FunctionalRole;
using Equinor.ProCoSys.IPO.Query.GetOutstandingIpos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.IPO.Query.GetOutstandingIPOs
{
    public class GetOutstandingIposQueryHandler : IRequestHandler<GetOutstandingIposQuery, Result<OutstandingIposResultDto>>
    {
        private readonly IReadOnlyContext _context;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IMainFunctionalRoleApiService _functionalRoleApiService;
        private readonly IPlantProvider _plantProvider;

        public GetOutstandingIposQueryHandler(
            IReadOnlyContext context,
            ICurrentUserProvider currentUserProvider,
            IMainFunctionalRoleApiService functionalRoleApiService,
            IPlantProvider plantProvider)
        {
            _context = context;
            _currentUserProvider = currentUserProvider;
            _functionalRoleApiService = functionalRoleApiService;
            _plantProvider = plantProvider;
        }

        public async Task<Result<OutstandingIposResultDto>> Handle(GetOutstandingIposQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();

            var currentUsersFunctionalRoleCodes =
                await _functionalRoleApiService.GetFunctionalRoleCodesByPersonOidAsync(_plantProvider.Plant,
                    currentUserOid.ToString());

            var completedInvitations = await (from i in _context.QuerySet<Invitation>()
                    .Include(ss => ss.Participants)
                where i.CompletedAtUtc.HasValue
                select i).ToListAsync(cancellationToken);

            var currentUsersOutstandingInvitations = new List<Invitation>();
            foreach (var invitation in completedInvitations)
            {
                if (UserWasInvitedAsPersonParticipant(invitation, currentUserOid))
                {
                    currentUsersOutstandingInvitations.Add(invitation);
                }
                else if(UserWasInvitedAsPersonInFunctionalRole(invitation, currentUsersFunctionalRoleCodes))
                {
                    currentUsersOutstandingInvitations.Add(invitation);
                }
            }

            var outstandingIposResultDto = new OutstandingIposResultDto(currentUsersOutstandingInvitations.Count,
                currentUsersOutstandingInvitations.Select(invitation => new OutstandingIpoDetailsDto
                {
                    InvitationId = invitation.Id,
                    Description = invitation.Description
                }));

            return new SuccessResult<OutstandingIposResultDto>(outstandingIposResultDto);
        }

        private static bool UserWasInvitedAsPersonParticipant(Invitation invitation, Guid currentUserOid) 
            => invitation.Participants.Any(p => p.AzureOid == currentUserOid);

        private static bool UserWasInvitedAsPersonInFunctionalRole(Invitation invitation, IEnumerable<string> currentUsersFunctionalRoleCodes)
        {
            var functionalRoleParticipantCodesOnInvitation = invitation.Participants.Where(p => p.SortKey == 1 &&
                                               p.FunctionalRoleCode != null &&
                                               p.Type == IpoParticipantType.FunctionalRole).Select(p => p.FunctionalRoleCode).ToList();

            return currentUsersFunctionalRoleCodes.Select(functionalRoleCode 
                => functionalRoleParticipantCodesOnInvitation.Contains(functionalRoleCode)).FirstOrDefault();
        }
    }
}
