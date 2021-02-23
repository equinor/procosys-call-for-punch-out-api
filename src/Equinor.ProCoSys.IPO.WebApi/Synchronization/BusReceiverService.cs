﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using System.Text.Json;
using Equinor.ProCoSys.IPO.Domain;
using Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate;
using Equinor.ProCoSys.BusReceiver;
using Equinor.ProCoSys.BusReceiver.Interfaces;
using Equinor.ProCoSys.BusReceiver.Topics;
using Equinor.ProCoSys.IPO.ForeignApi.MainApi.McPkg;
using Equinor.ProCoSys.IPO.WebApi.Telemetry;
using Fusion.Integration.Meeting;

namespace Equinor.ProCoSys.IPO.WebApi.Synchronization
{
    public class BusReceiverService : IBusReceiverService
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IPlantSetter _plantSetter;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelemetryClient _telemetryClient;
        private readonly IReadOnlyContext _context;
        private readonly IFusionMeetingClient _meetingClient;
        private readonly IMcPkgApiService _mcPkgApiService;
        private const string IpoBusReceiverTelemetryEvent = "IPO Bus Receiver";

        public BusReceiverService(
            IInvitationRepository invitationRepository,
            IPlantSetter plantSetter,
            IUnitOfWork unitOfWork,
            ITelemetryClient telemetryClient,
            IReadOnlyContext context,
            IFusionMeetingClient meetingClient,
            IMcPkgApiService mcPkgApiService)
        {
            _invitationRepository = invitationRepository;
            _plantSetter = plantSetter;
            _unitOfWork = unitOfWork;
            _telemetryClient = telemetryClient;
            _context = context;
            _meetingClient = meetingClient;
            _mcPkgApiService = mcPkgApiService;
        }

        public async Task ProcessMessageAsync(PcsTopic pcsTopic, Message message, CancellationToken token)
        {
            var messageJson = Encoding.UTF8.GetString(message.Body);

            switch (pcsTopic)
            {
                case PcsTopic.Ipo:
                    await ProcessIpoEvent(messageJson);
                    break;
                case PcsTopic.Project:
                    ProcessProjectEvent(messageJson);
                    break;
                case PcsTopic.CommPkg:
                    ProcessCommPkgEvent(messageJson);
                    break;
                case PcsTopic.McPkg:
                    ProcessMcPkgEvent(messageJson);
                    break;
            }
            await _unitOfWork.SaveChangesAsync(token);
        }

        private void ProcessMcPkgEvent(string messageJson)
        {
            var mcPkgEvent = JsonSerializer.Deserialize<McPkgTopic>(messageJson);

            _telemetryClient.TrackEvent(IpoBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {BusReceiverTelemetryConstants.Event, mcPkgEvent.TopicName},
                    {BusReceiverTelemetryConstants.McPkgNo, mcPkgEvent.McPkgNo},
                    {BusReceiverTelemetryConstants.ProjectSchema, mcPkgEvent.ProjectSchema[4..]},
                    {BusReceiverTelemetryConstants.ProjectName, mcPkgEvent.ProjectName.Replace('$', '_')}
                });
            _plantSetter.SetPlant(mcPkgEvent.ProjectSchema);
            _invitationRepository.UpdateMcPkgOnInvitations(mcPkgEvent.ProjectName, mcPkgEvent.McPkgNo, mcPkgEvent.Description);
        }

        private void ProcessCommPkgEvent(string messageJson)
        {
            var commPkgEvent = JsonSerializer.Deserialize<CommPkgTopic>(messageJson);

            _telemetryClient.TrackEvent(IpoBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {BusReceiverTelemetryConstants.Event, commPkgEvent.TopicName},
                    {BusReceiverTelemetryConstants.CommPkgNo, commPkgEvent.CommPkgNo},
                    {BusReceiverTelemetryConstants.ProjectSchema, commPkgEvent.ProjectSchema[4..]},
                    {BusReceiverTelemetryConstants.ProjectName, commPkgEvent.ProjectName.Replace('$', '_')}
                });
            _plantSetter.SetPlant(commPkgEvent.ProjectSchema);
            _invitationRepository.UpdateCommPkgOnInvitations(commPkgEvent.ProjectName, commPkgEvent.CommPkgNo,
                commPkgEvent.Description);
        }

        private void ProcessProjectEvent(string messageJson)
        {
            var projectEvent = JsonSerializer.Deserialize<ProjectTopic>(messageJson);
            _telemetryClient.TrackEvent(IpoBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {BusReceiverTelemetryConstants.Event, projectEvent.TopicName},
                    {BusReceiverTelemetryConstants.ProjectSchema, projectEvent.ProjectSchema[4..]},
                    {BusReceiverTelemetryConstants.ProjectName, projectEvent.ProjectName.Replace('$', '_')}
                });
            _plantSetter.SetPlant(projectEvent.ProjectSchema);
            _invitationRepository.UpdateProjectOnInvitations(projectEvent.ProjectName, projectEvent.Description);
        }

        private async Task ProcessIpoEvent(string messageJson)
        {
            var ipoEvent = JsonSerializer.Deserialize<IpoTopic>(messageJson);
            _telemetryClient.TrackEvent(IpoBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {BusReceiverTelemetryConstants.Event, ipoEvent.TopicName},
                    {BusReceiverTelemetryConstants.ProjectSchema, ipoEvent.ProjectSchema[4..]},
                    {BusReceiverTelemetryConstants.Ipo, ipoEvent.InvitationGuid},
                    {BusReceiverTelemetryConstants.IpoEvent, ipoEvent.Event}
                });
            _plantSetter.SetPlant(ipoEvent.ProjectSchema);
            var invitation = _context.QuerySet<Invitation>()
                .SingleOrDefault(i => i.ObjectGuid == Guid.Parse(ipoEvent.InvitationGuid));
            if (invitation == null)
            {
                throw new Exception($"Invitation {ipoEvent.InvitationGuid} not found");
            }

            if (ipoEvent.Event == "Canceled")
            {
                await ClearM01DatesAndCancelMeeting(ipoEvent, invitation);
            }
            else if (ipoEvent.Event == "Completed")
            {
                await SetM01Dates(ipoEvent, invitation);
            }
        }

        private async Task SetM01Dates(IpoTopic ipoEvent, Invitation invitation)
        {
            try
            {
                await _mcPkgApiService.SetM01DatesAsync(
                    ipoEvent.ProjectSchema,
                    invitation.Id,
                    invitation.ProjectName,
                    invitation.McPkgs.Select(mcPkg => mcPkg.McPkgNo).ToList(),
                    invitation.CommPkgs.Select(c => c.CommPkgNo).ToList());
            }
            catch (Exception e)
            {
                throw new Exception($"Error: Could not set M-01 dates for {invitation.ObjectGuid}", e);
            }
        }

        private async Task ClearM01DatesAndCancelMeeting(IpoTopic ipoEvent, Invitation invitation)
        {
            if (ipoEvent.Status == (int) IpoStatus.Completed)
            {
                try
                {
                    await _mcPkgApiService.ClearM01DatesAsync(
                        ipoEvent.ProjectSchema,
                        invitation.Id,
                        invitation.ProjectName,
                        invitation.McPkgs.Select(mcPkg => mcPkg.McPkgNo).ToList(),
                        invitation.CommPkgs.Select(c => c.CommPkgNo).ToList());
                }
                catch (Exception e)
                {
                    throw new Exception($"Error: Could not clear M-01 dates for {invitation.ObjectGuid}", e);
                }
            }

            try
            {
                await _meetingClient.DeleteMeetingAsync(invitation.MeetingId);
            }
            catch (Exception e)
            {
                throw new Exception($"Error: Could not cancel outlook meeting {invitation.MeetingId}.", e);
            }
        }
    }
}
