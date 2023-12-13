using Blazored.LocalStorage;
using BlogApp.BlazorServer.Data;
using BlogApp.BlazorServer.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization; // Add this for authentication state handling
using Microsoft.AspNetCore.Identity; // If using ASP.NET Core Identity
using Microsoft.EntityFrameworkCore; // If using Entity Framework
using Microsoft.JSInterop;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// Server timeout for SignalR increase
builder.Services.AddServerSideBlazor().AddHubOptions(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
});

// Add ASP.NET Core Identity Services (if using Identity)
// Configure Entity Framework and Identity here if applicable

// Add authentication and authorization services
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Add Scoped Authentication State Provider
//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>(); // Replace with your custom provider if applicable
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>(provider =>
    new CustomAuthenticationStateProvider(provider.GetRequiredService<IJSRuntime>()));

builder.Services.AddSingleton<WeatherForecastService>();

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
app.MapFallbackToPage("/_Host");

app.Run();