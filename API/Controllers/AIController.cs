using BarberBooking.Application.AI.Queries;
using BarberBooking.Contracts.AI;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BarberBooking.API.Controllers;

[ApiController]
[Route("api/ai")]
public class AIController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AIController> _logger;

    public AIController(IMediator mediator, ILogger<AIController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("reserve")]
    public async Task<ActionResult<VoiceCommandResponse>> ProcessVoice([FromBody] VoiceCommandRequest request)
    {
        _logger.LogWarning(
            "AI reserve called. transcriptLen={TranscriptLen} barberId={BarberId} duration={Duration} previousResponseIdPresent={HasPrev}.",
            request.Transcript?.Length ?? 0,
            request.BarberId,
            request.ServiceDurationMinutes,
            !string.IsNullOrWhiteSpace(request.PreviousResponseId));

        var result = await _mediator.Send(new ProcessVoiceCommandQuery(
            request.Transcript,
            request.Context ?? "Main Screen",
            request.BarberId,
            request.ServiceDurationMinutes,
            request.PreviousResponseId));

        return Ok(result);
    }
}

public record VoiceCommandRequest(
    string Transcript,
    string? Context,
    Guid? BarberId,
    int? ServiceDurationMinutes,
    string? PreviousResponseId);
