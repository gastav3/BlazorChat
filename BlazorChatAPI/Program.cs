using BlazorChatAPI.Data;
using BlazorChatAPI.Hubs;
using BlazorChatAPI.Repositories;
using BlazorChatAPI.Services;
using BlazorChatShared.Mapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// CORS: Allow Blazor WASM running on https://localhost:7081
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("https://localhost:7081")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Only if you're using auth like SignalR/cookies
    });
});

builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<BlazorChatProfile>());

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

var app = builder.Build();

app.MapHub<ChatHub>("/chathub");

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS before routing or authorization
app.UseCors("AllowLocalhost");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
