using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwipeMate.Application.Common.Models; // PagedResult<>, Result
using SwipeMate.Application.Features.Messaging; // ConversationDto, MessageDto
using SwipeMate.Application.Features.Messaging.Commands.SendMessage;
using SwipeMate.Application.Features.Messaging.Queries.GetConversations;
using SwipeMate.Application.Features.Messaging.Queries.GetMessages;

namespace SwipeMate.Api.Controllers;

[ApiController]
[Route("api/v1/conversations")]
[Authorize(Policy = "Member")] // diagram: [Authorize] member policy

public class MessagesController : ControllerBase
{
    private readonly GetMessagesQueryHandler _getMessages;
    private readonly GetConversationsQueryHandler _getConversations;
    private readonly SendMessageCommandHandler _sendMessage;

    // solid arrows = constructor dependencies
    public MessagesController(
        GetConversationsQueryHandler getConversations,
        GetMessagesQueryHandler getMessages,
        SendMessageCommandHandler sendMessage)
    {
        _getConversations = getConversations;
        _getMessages = getMessages;
        _sendMessage = sendMessage;
    }

    // GET /api/v1/conversations?cursor=...

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ConversationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ConversationDto>>> GetConversations(
        [FromQuery] GetConversationsQuery query,
        CancellationToken ct)
    {
        var result = await _getConversations.Handle(query, ct);
        return Ok(result);
    }

    // GET /api/v1/conversations/{conversationId}/messages?before=...
    [HttpGet("{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(PagedResult<MessageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<MessageDto>>> GetMessages(
        [FromRoute] Guid conversationId,
        [FromQuery] GetMessagesQuery query,
        CancellationToken ct)
    {
        // ensure the query knows which conversation
        query = query with { ConversationId = conversationId };

        var result = await _getMessages.Handle(query, ct);
        return Ok(result);
    }

    // POST /api/v1/conversations/{conversationId}/messages
    [HttpPost("{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<MessageDto>> SendMessage(
        [FromRoute] Guid conversationId,
        [FromBody] SendMessageCommand command,
        CancellationToken ct)
    {
        // enforce route-id wins over body
        command = command with { ConversationId = conversationId };

        var message = await _sendMessage.Handle(command, ct);
        return CreatedAtAction(
            nameof(GetMessages),
            new { conversationId },
            message);
    }
}
