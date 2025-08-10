using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;
using System.Net.Http.Json;

namespace BlazorChatWeb.WebServices;

public class ChatRoomWebService : IChatRoomWebService
{
    private readonly HttpClient _httpClient;

    public ChatRoomWebService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Room>> GetAllRooms()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<Room>>("api/chatroom");
            return result ?? [];
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching rooms: {ex.Message}");
            return new();
        }
    }

    public async Task<Room?> GetRoomById(string id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Room>($"api/chatroom/{id}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching room {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Room?> CreateRoom(CreateRoomParameter request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/chatroom", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Room>();
            }
            else
            {
                Console.Error.WriteLine($"Create room failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating room: {ex.Message}");
        }

        return null;
    }
}