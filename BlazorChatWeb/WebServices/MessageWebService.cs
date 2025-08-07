using BlazorChatShared.Models.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace BlazorChatWeb.WebServices;

public class MessageWebService : IMessageWebService
{
    private readonly HttpClient _httpClient;

    public MessageWebService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<ChatMessage>> GetChatMessagesByRoomId(string id)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<ChatMessage>>("api/ChatMessage");
            return result ?? [];
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching messages: {ex.Message}");
            return new();
        }
    }
}
