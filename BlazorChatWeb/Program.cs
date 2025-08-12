using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorChatWeb;
using BlazorChatWeb.WebServices;
using BlazorChatWeb.Hub;
using BlazorChatWeb.StateServices;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
var hubUrl = builder.Configuration["SignalRHubUrl"];
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl!)
});


builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IChatHubService, ChatHubService>();
builder.Services.AddScoped<IChatRoomWebService, ChatRoomWebService>();
builder.Services.AddScoped<IRoomState, RoomState>();

await builder.Build().RunAsync();
