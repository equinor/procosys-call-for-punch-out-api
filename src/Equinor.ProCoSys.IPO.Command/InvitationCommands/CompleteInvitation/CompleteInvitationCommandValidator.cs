﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.Command.Validators.InvitationValidators;
using Equinor.ProCoSys.IPO.Command.Validators.RowVersionValidators;
using FluentValidation;

namespace Equinor.ProCoSys.IPO.Command.InvitationCommands.CompleteInvitation
{
    public class CompleteInvitationCommandValidator : AbstractValidator<CompleteInvitationCommand>
    {
        public CompleteInvitationCommandValidator(IInvitationValidator invitationValidator, IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingInvitation(command.InvitationId, token))
                .WithMessage(command =>
                    $"IPO with this ID does not exist! Id={command.InvitationId}")
                .MustAsync((command, token) => BeAnInvitationInPlannedStage(command.InvitationId, token))
                .WithMessage(command =>
                    "Invitation is not in planned stage, and thus cannot be completed!")
                .Must((command) => HaveAValidRowVersion(command.InvitationRowVersion))
                .WithMessage(command =>
                    $"Invitation row version is not valid! InvitationRowVersion={command.InvitationRowVersion}")
                .Must(command => HaveAValidRowVersion(command.ParticipantRowVersion))
                .WithMessage(command =>
                    $"Participant row version is not valid! ParticipantRowVersion={command.ParticipantRowVersion}")
                .MustAsync((command, token) => BeAContractorOnIpo(command.InvitationId, token))
                .WithMessage(command =>
                    "The IPO does not have a contractor assigned to complete the IPO!")
                .MustAsync((command, token) => BeTheAssignedPersonIfPersonParticipant(command.InvitationId, token))
                .WithMessage(command =>
                    "Person signing is not the contractor assigned to complete this IPO, or there is not a valid functional role on the IPO!");

            async Task<bool> BeAnExistingInvitation(int invitationId, CancellationToken token)
                => await invitationValidator.IpoExistsAsync(invitationId, token);

            async Task<bool> BeAnInvitationInPlannedStage(int invitationId, CancellationToken token)
                => await invitationValidator.IpoIsInPlannedStageAsync(invitationId, token);

            async Task<bool> BeAContractorOnIpo(int invitationId, CancellationToken token)
                => await invitationValidator.ContractorExistsAsync(invitationId, token);

            async Task<bool> BeTheAssignedPersonIfPersonParticipant(int invitationId, CancellationToken token)
                => await invitationValidator.ValidContractorParticipantExistsAsync(invitationId, token);

            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
