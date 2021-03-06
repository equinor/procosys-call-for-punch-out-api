﻿using System;
using Equinor.ProCoSys.IPO.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.IPO.Domain.Audit;
using Equinor.ProCoSys.IPO.Domain.Time;

namespace Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate
{
    public class McPkg : PlantEntityBase, ICreationAuditable
    {
        public const int McPkgNoMaxLength = 30;
        public const int SystemMaxLength = 40;

        protected McPkg()
            : base(null)
        {
        }

        public McPkg(string plant, string projectName, string commPkgNo, string mcPkgNo, string description, string system)
            : base(plant)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException(nameof(projectName));
            }
            if (string.IsNullOrEmpty(commPkgNo))
            {
                throw new ArgumentNullException(nameof(commPkgNo));
            }
            if (string.IsNullOrEmpty(system))
            {
                throw new ArgumentNullException(nameof(system));
            }
            if (!system.Contains('|') || system.Length < 3)
            {
                throw new ArgumentException($"{(nameof(system))} is not valid. Must be at least three characters and include '|'");
            }
            if (string.IsNullOrEmpty(mcPkgNo))
            {
                throw new ArgumentNullException(nameof(mcPkgNo));
            }
            ProjectName = projectName;
            CommPkgNo = commPkgNo;
            System = system;
            Description = description;
            McPkgNo = mcPkgNo;
        }

        public string ProjectName { get; private set; }
        public string CommPkgNo { get; private set; }
        public string Description { get; set; }
        public string McPkgNo { get; private set; }
        public string System { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;
        }

        public void MoveToCommPkg(string toCommPkgNo) => CommPkgNo = toCommPkgNo;

        public void Rename(string toMcPkgNo) => McPkgNo = toMcPkgNo;

        public void MoveToProject(string toProject) => ProjectName = toProject;

        public string SystemSubString => System.Substring(0, System.LastIndexOf('|'));
    }
}
