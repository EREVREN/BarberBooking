using BarberBooking.Application.AI.Queries;
using BarberBooking.Application.Availability.Queries;
using BarberBooking.Contracts.AI;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace BarberBooking.Application.AI.Handlers;

public sealed class ProcessVoiceCommandHandler : IRequestHandler<ProcessVoiceCommandQuery, VoiceCommandResponse>
{
    private readonly IAIMessageGenerator _aiGenerator;
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessVoiceCommandHandler> _logger;

    public ProcessVoiceCommandHandler(
        IAIMessageGenerator aiGenerator,
        IMediator mediator,
        ILogger<ProcessVoiceCommandHandler> logger)
    {
        _aiGenerator = aiGenerator;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<VoiceCommandResponse> Handle(ProcessVoiceCommandQuery request, CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "ProcessVoiceCommand start. transcriptLen={TranscriptLen} prevResponseIdPresent={HasPrev} barberId={BarberId} duration={Duration}.",
            request.Transcript?.Length ?? 0,
            !string.IsNullOrWhiteSpace(request.PreviousResponseId),
            request.BarberId,
            request.ServiceDurationMinutes);

        var ai = await _aiGenerator.ProcessVoiceCommandAsync(
            request.Transcript,
            request.Context,
            request.PreviousResponseId);

        _logger.LogWarning(
            "ProcessVoiceCommand AI done. responseIdPresent={HasResponseId} scheduledTime={ScheduledTime}.",
            !string.IsNullOrWhiteSpace(ai.ResponseId),
            ai.ScheduledTime);

        static bool IsAiParseFailure(VoiceCommandResponse response)
        {
            if (response.Steps.Count == 1 &&
                string.Equals(response.Steps[0], "Error Parsing", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return response.SpokenResponse.Contains("couldn't process", StringComparison.OrdinalIgnoreCase);
        }

        var requested = ResolveRequestedDateTime(
            request.Transcript,
            ai.ScheduledTime,
            now: DateTime.Now);

        if (requested == null)
        {
            // No concrete time detected; return the AI response as-is.
            return ai;
        }

        // Only set a concrete scheduled time once we have checked availability (when enough inputs are provided).
        if (request.BarberId is not Guid barberId || barberId == Guid.Empty ||
            request.ServiceDurationMinutes is not int durationMinutes || durationMinutes <= 0)
        {
            return ai with { ScheduledTime = null };
        }

        var availability = await _mediator.Send(
            new GetAvailabilityQuery(barberId, requested.Value.Date, durationMinutes),
            cancellationToken);

        static DateTime TruncateToMinute(DateTime value) =>
            new(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Kind);

        var requestedMinute = TruncateToMinute(requested.Value);

        var exact = availability.Slots.FirstOrDefault(s =>
            s.isAvailable && TruncateToMinute(s.Start) == requestedMinute);

        if (exact != null)
        {
            if (IsAiParseFailure(ai))
            {
                return ai with
                {
                    SpokenResponse = $"That time is available: {exact.Start:f}. Confirm the booking?",
                    Steps = new List<string> { "Confirm booking", $"Schedule at {exact.Start:o}" },
                    ScheduledTime = exact.Start
                };
            }

            return ai with { ScheduledTime = exact.Start };
        }

        var first = availability.Slots.FirstOrDefault(s => s.isAvailable);
        if (first != null)
        {
            if (IsAiParseFailure(ai))
            {
                return ai with
                {
                    SpokenResponse = $"That time isn't available. Next available is {first.Start:f}.",
                    Steps = new List<string> { "Offer next available slot", $"Schedule at {first.Start:o}" },
                    ScheduledTime = first.Start
                };
            }

            var spoken = $"{ai.SpokenResponse} That time isn't available. Next available is {first.Start:f}.";
            return ai with { SpokenResponse = spoken, ScheduledTime = first.Start };
        }

        return ai with { ScheduledTime = null };
    }

    private static DateTime? ResolveRequestedDateTime(string transcript, DateTime? aiScheduledTime, DateTime now)
    {
        // Prefer a simple deterministic parse for "today/tomorrow at HH[:mm][am/pm]".
        var lowered = transcript.ToLowerInvariant();
        var isTomorrow = lowered.Contains("tomorrow");
        var isToday = lowered.Contains("today");

        if (isTomorrow || isToday)
        {
            var match = Regex.Match(lowered, @"\b(?<h>\d{1,2})(:(?<m>\d{2}))?\s*(?<ampm>am|pm)?\b");
            if (match.Success &&
                int.TryParse(match.Groups["h"].Value, out var hour) &&
                int.TryParse(match.Groups["m"].Success ? match.Groups["m"].Value : "0", out var minute))
            {
                var ampm = match.Groups["ampm"].Value;
                if (ampm == "pm" && hour < 12) hour += 12;
                if (ampm == "am" && hour == 12) hour = 0;

                if (hour is >= 0 and <= 23 && minute is >= 0 and <= 59)
                {
                    var date = now.Date.AddDays(isTomorrow ? 1 : 0);
                    var resolved = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0, now.Kind);
                    if (resolved >= now.AddMinutes(-1))
                    {
                        return resolved;
                    }
                }
            }
        }

        // Otherwise accept the model time only if it's in the future.
        if (aiScheduledTime != null && aiScheduledTime.Value >= now.AddMinutes(-1))
        {
            return aiScheduledTime.Value;
        }

        return null;
    }
}
