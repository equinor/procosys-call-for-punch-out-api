﻿using System;
using MediatR;

namespace Equinor.Procosys.IPO.Domain.Events
{
    public class IpoSignedEvent : INotification
    {
        public IpoSignedEvent(
            string plant,
            Guid objectGuid)
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
