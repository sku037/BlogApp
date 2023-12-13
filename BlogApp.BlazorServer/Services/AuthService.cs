using System.Net.Http;
using System.Threading.Tasks;
using BlogApp.BlazorServer.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using System;
using Microsoft.AspNetCore.Components.Authorization;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;
    private readonly NavigationManager _navigationManager;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthService(IHttpClientFactory httpClientFactory,
                       ILocalStorageService localStorageService,
                       NavigationManager navigationManager,
                       AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClientFactory.CreateClient("BlogApi");
        _localStorageService = localStorageService;
        _navigationManager = navigationManager;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<bool> Login(LoginModel loginModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginModel);

        if (response.IsSuccessStatusCode)
        {
            var tokenResult = await response.Content.ReadFromJsonAsync<TokenResult>();
            if (tokenResult?.Token != null)
            {
                await _localStorageService.SetItemAsync("authToken", tokenResult.Token);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", tokenResult.Token);
                ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.Username);
                return true;
            }
        }

        return false;
    }

    public async Task Logout()
    {
        await _localStorageService.RemoveItemAsync("authToken");
        _httpClient.DefaultRequestHeaders.Authorization = null;
        ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _navigationManager.NavigateTo("/login", true);
    }

    public async Task<bool> IsUserLoggedIn()
    {
        var token = await _localStorageService.GetItemAsync<string>("authToken");
        return !string.IsNullOrEmpty(token);
    }

    // Get token for username in welcome
    public async Task<string> GetAuthToken()
    {
        return await _localStorageService.GetItemAsync<string>("authToken");
    }

    public async Task<bool> RegisterUser(RegisterModel registerModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", registerModel);

        if (response.IsSuccessStatusCode)
        {
            // Optionally log the user in directly after registration
            return true;
        }

        // Handle errors here
        return false;
    }
}

public class TokenResult
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}
