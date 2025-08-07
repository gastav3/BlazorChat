using AutoMapper;
using BlazorChatAPI.Services;
using BlazorChatShared.Models.Entities;
using BlazorChatShared.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorChatAPI.Controllers;

public class ChatMessageController : Controller
{
    private readonly IMapper _autoMapper;
    private readonly IChatService _chatService;
    public ChatMessageController(IMapper autoMapper, IChatService chatService)
    {
        _autoMapper = autoMapper ?? throw new ArgumentNullException(nameof(autoMapper));
        _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
    }

    [HttpGet]
    public async Task<ActionResult<List<ChatMessage>>> GetMessagesByRoomId(string id)
    {
        var messagesEntity = await _chatService.GetMessagesByRoomId(id, 10 , 10);
        var messages = _autoMapper.Map<IEnumerable<ChatMessage>>(messagesEntity);
        return Ok(messages);
    }
}
