using BlazorChatShared.Models.Models;
using BlazorChatWeb.Hub;
using BlazorChatWeb.StateServices;
using BlazorChatWeb.WebServices;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

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
    private IRoomState _roomState { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;
    private const string VisitedRoomsKey = "visitedRooms";

    private ChatMessage message = default!;
    private List<ChatMessage> messages = [];

    private ElementReference messageListDiv;

    public Room LoadedRoom { get; set; } = default!;

    private int _currentPage = 1;
    private const int _pageSize = 20;
    private bool _hasMoreMessages = true;
    private bool _isLoading = false;
    private bool _shouldScrollToBottom = true;

    private Func<ChatMessage, Task>? _messageReceivedHandler;
    private Func<Room, Task>? _roomReceivedHandler;

    private int _scrollPos = default!;

    protected override async Task OnInitializedAsync()
    {
        _messageReceivedHandler = async (msg) =>
        {
            if (msg.GroupId != Id || messages.Any(x => x.Id == msg.Id))
            {
                return;
            }

            messages.Add(msg);
            await InvokeAsync(StateHasChanged);
            await DoScrollToBottom();
        };

        chatHubService.OnMessageReceived += _messageReceivedHandler;

        _roomReceivedHandler = async (updatedRoom) =>
        {
            if (LoadedRoom != null && LoadedRoom.Id == updatedRoom.Id)
            {
                LoadedRoom.Connections = updatedRoom.Connections;
                LoadedRoom.Name = updatedRoom.Name;

                await InvokeAsync(StateHasChanged);
                return;
            }
        };

        chatHubService.OnRoomReceived += _roomReceivedHandler;

        await LoadChatMessages();

        await InvokeAsync(StateHasChanged);
        await DoScrollToBottom();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            throw new ArgumentException("Room ID cannot be null or empty.", nameof(Id));
        }

        messages.Clear();
        _currentPage = 1;
        _hasMoreMessages = true;
        _isLoading = false;
        _shouldScrollToBottom = true;

        LoadedRoom = await _chatRoomWebService.GetRoomById(Id) ?? throw new InvalidOperationException($"Room with Id '{Id}' was not found.");
        _roomState.SelectedRoom = LoadedRoom;

        message = new ChatMessage
        {
            GroupId = Id,
            User = "test user", // Replace
            Message = string.Empty
        };

        await chatHubService.StartConnection("https://localhost:7199/chathub");

        if (chatHubService.Connection != null)
        {
            await chatHubService.Connection.InvokeAsync("JoinRoom", Id);
        }

        await LoadChatMessages();

        await InvokeAsync(StateHasChanged);
        await DoScrollToBottom();

        await SaveVisitedRoomAsync(LoadedRoom);
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

    private async Task SaveVisitedRoomAsync(Room room)
    {
        var storedRooms = await LocalStorage.GetItemAsync<List<Room>>(VisitedRoomsKey) ?? new List<Room>();

        storedRooms.RemoveAll(r => r.Id == room.Id);
        storedRooms.Insert(0, room);

        //Keep only latest 10
        if (storedRooms.Count > 10)
            storedRooms = storedRooms.Take(10).ToList();

        await LocalStorage.SetItemAsync(VisitedRoomsKey, storedRooms);
    }
    public async ValueTask DisposeAsync()
    {
        if (_messageReceivedHandler != null)
        {
            chatHubService.OnMessageReceived -= _messageReceivedHandler;
        }

        if (_roomReceivedHandler != null)
        {
            chatHubService.OnRoomReceived -= _roomReceivedHandler;
        }

        if (chatHubService?.Connection != null)
        {
            await chatHubService.Connection.InvokeAsync("LeaveRoom", Id);
        }
    }
}
