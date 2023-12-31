using Blazored.LocalStorage;
using BlogApp.BlazorServer.Data;
using BlogApp.BlazorServer.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization; // Add this for authentication state handling
using Microsoft.AspNetCore.Identity; // If using ASP.NET Core Identity
using Microsoft.EntityFrameworkCore; // If using Entity Framework
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/chatapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddRazorPages();
// Server timeout for SignalR increase
builder.Services.AddServerSideBlazor().AddHubOptions(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
});

// SignalR Hub Route
builder.Services.AddSignalR();

// Add ASP.NET Core Identity Services (if using Identity)
// Configure Entity Framework and Identity here if applicable

// Add authentication and authorization services
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();


builder.Services.AddSingleton<WeatherForecastService>();
// service for saving chat to file
builder.Services.AddSingleton(new ChatHistoryService("chat_history.txt"));


// Add HTTP client to send requests to Blog.WebApi
builder.Services.AddHttpClient("BlogApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7212");
});

builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddSingleton<IAuthorizationHandler, ResourceOwnerAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsResourceOwner", policy =>
        policy.Requirements.Add(new ResourceOwnerRequirement()));
});

// Detailed error messages enabled 
builder.Services.AddServerSideBlazor().AddHubOptions(options =>
{
    options.EnableDetailedErrors = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapBlazorHub();
app.MapHub<ChatHub>("/chatHub"); // Map the ChatHub to "/chatHub" endpoint
app.MapFallbackToPage("/_Host");

app.Run();