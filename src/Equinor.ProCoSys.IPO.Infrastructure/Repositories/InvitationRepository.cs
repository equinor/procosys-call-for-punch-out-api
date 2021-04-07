﻿using System.Linq;
using Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.IPO.Infrastructure.Repositories
{
    public class InvitationRepository : RepositoryBase<Invitation>, IInvitationRepository
    {
        public InvitationRepository(IPOContext context)
            : base(context, context.Invitations, 
                context.Invitations
                    .Include(x => x.Participants)
                    .Include(x => x.McPkgs)
                    .Include(x => x.CommPkgs)
                    .Include(x => x.Comments)
                    .Include(x => x.Attachments))
        {
        }

        public void UpdateProjectOnInvitations(string projectName, string description)
        {
            //Intentionally left blank for now
        }

        public void UpdateCommPkgOnInvitations(string projectName, string commPkgNo, string description)
        {
            var commPkgsToUpdate = _context.CommPkgs.Where(cp => cp.ProjectName == projectName && cp.CommPkgNo == commPkgNo).ToList();

            commPkgsToUpdate.ForEach(cp => cp.Description = description);
        }

        public void MoveCommPkg(string fromProject, string toProject, string commPkgNo, string description)
        {
            var commPkgsToMove = _context.CommPkgs.Where(cp => cp.ProjectName == fromProject && cp.CommPkgNo == commPkgNo).ToList();
            var mcPkgsToMove = _context.McPkgs.Where(mc => mc.ProjectName == fromProject && mc.CommPkgNo == commPkgNo).ToList();

            var invitationsToMove =
                _context.Invitations
                    .Where(i => i.ProjectName == fromProject &&
                                (i.CommPkgs.Any(c => c.CommPkgNo == commPkgNo) || i.McPkgs.Any(m => m.CommPkgNo == commPkgNo))).ToList();

            invitationsToMove.ForEach(i =>
            {
                i.MoveToProject(toProject);
            });

            commPkgsToMove.ForEach(cp =>
            {
                cp.Description = description;
                cp.MoveToProject(toProject);
            });

            mcPkgsToMove.ForEach(mc =>
            {
                mc.MoveToProject(toProject);
            });

        }

        public void UpdateMcPkgOnInvitations(string projectName, string mcPkgNo, string description)
        {
            var mcPkgsToUpdate = _context.McPkgs.Where(mp => mp.ProjectName == projectName && mp.McPkgNo == mcPkgNo).ToList();

            mcPkgsToUpdate.ForEach(mp => mp.Description=description);
        }

        public void RemoveParticipant(Participant participant)
            => _context.Participants.Remove(participant);

        public void RemoveAttachment(Attachment attachment)
            => _context.Attachments.Remove(attachment);
    }
}
