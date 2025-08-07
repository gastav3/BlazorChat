using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;
using BlazorChatWeb.Hub;
using BlazorChatWeb.WebServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChatWeb.Pages;

public partial class ChatRoom : ComponentBase, IAsyncDisposable
{
    [Parameter]
    public string Id { get; set; } = default!;

    [Inject]
    private IChatHubService chatHubService { get; set; } = default!;

    [Inject]
    private IChatRoomWebService _chatRoomWebService { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private ChatMessage message = default!;
    private List<ChatMessage> messages = new();

    private ElementReference messageListDiv;

    public Room LoadedRoom { get; set; } = default!;

    private int _currentPage = 1;
    private const int _pageSize = 20;
    private bool _hasMoreMessages = true;
    private bool _isLoading = false;
    private bool _shouldScrollToBottom = true;
    private Func<ChatMessage, Task>? _messageReceivedHandler;

    private int _scrollPos = default!;

    protected override async Task OnInitializedAsync()
    {
        await chatHubService.StartConnection("https://localhost:7199/chathub");

        _messageReceivedHandler = async (msg) =>
        {
            if (msg.GroupId != Id || messages.Any(x => x.Id == msg.Id))
                return;

            messages.Add(msg);
            await InvokeAsync(StateHasChanged);
            await DoScrollToBottom();
        };  

        chatHubService.OnMessageReceived += _messageReceivedHandler;

        await LoadChatMessages();

        await InvokeAsync(StateHasChanged);
        await DoScrollToBottom();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrWhiteSpace(Id))
            throw new ArgumentException("Room ID cannot be null or empty.", nameof(Id));

        await chatHubService.StartConnection("https://localhost:7199/chathub");

        if (chatHubService.Connection != null)
        {
            await chatHubService.Connection.InvokeAsync("JoinRoom", Id);
        }

        messages.Clear();
        _currentPage = 1;
        _hasMoreMessages = true;
        _isLoading = false;
        _shouldScrollToBottom = true;

        LoadedRoom = await _chatRoomWebService.GetRoomByIdAsync(Id)
            ?? throw new InvalidOperationException($"Room with Id '{Id}' was not found.");

        message = new ChatMessage
        {
            GroupId = Id,
            User = "test user", // Replace with your auth user
            Message = string.Empty
        };

        await LoadChatMessages();

        await InvokeAsync(StateHasChanged);
        await DoScrollToBottom();
    }

    private async Task DoScrollToBottom()
    {
        if (_shouldScrollToBottom)
        {
            await ScrollToBottom();
            _shouldScrollToBottom = false;
        }
    }

    private async Task OnScrollAsync()
    {
        _shouldScrollToBottom = await IsAtBottomAsync();

        var scrollTop = await JSRuntime.InvokeAsync<int>("getScrollTop", messageListDiv);

        if (scrollTop == 0 && _hasMoreMessages && !_isLoading)
        {
            _scrollPos = scrollTop;
            _isLoading = true;
            _currentPage++;
            await LoadChatMessages();
        }
    }

    private async Task LoadChatMessages()
    {
        var prevScrollHeight = await JSRuntime.InvokeAsync<int>("getScrollHeight", messageListDiv);

        var result = await chatHubService.LoadMessages(Id, _currentPage, _pageSize);

        if (result?.Messages != null && result.Messages.Any())
        {
            var newMessages = result.Messages
                .Where(m => !messages.Any(existing => existing.Id == m.Id))
                .ToList();

            messages.InsertRange(0, newMessages);
        }

        _hasMoreMessages = result?.HasMore ?? false;
        _isLoading = false;

        await InvokeAsync(StateHasChanged);

        var newScrollHeight = await JSRuntime.InvokeAsync<int>("getScrollHeight", messageListDiv);
        var scrollDifference = newScrollHeight - prevScrollHeight;

        await ScrollToPosition(scrollDifference);
    }


    private async Task ClickSend()
    {
        if (!string.IsNullOrWhiteSpace(message.Message))
        {
            await chatHubService.SendMessage(message);
            message.Message = string.Empty;
        }
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey)
        {
            await ClickSend();
        }
    }

    private async Task<bool> IsAtBottomAsync()
    {
        return await JSRuntime.InvokeAsync<bool>("isScrolledToBottom", messageListDiv);
    }

    private async Task ScrollToBottom()
    {
        await JSRuntime.InvokeVoidAsync("scrollToBottom", messageListDiv);
    }

    private async Task ScrollToPosition(int pos)
    {
        await JSRuntime.InvokeVoidAsync("scrollToPosition", messageListDiv, pos);
    }


    public async ValueTask DisposeAsync()
    {
        if (_messageReceivedHandler != null)
        {
            chatHubService.OnMessageReceived -= _messageReceivedHandler;
        }

        if (chatHubService?.Connection != null)
        {
            await chatHubService.Connection.InvokeAsync("LeaveRoom", Id);
        }
    }
}
