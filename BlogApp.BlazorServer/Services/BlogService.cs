using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Json;
using BlogApp.BlazorServer.Models;
using Microsoft.Extensions.Http;

namespace BlogApp.BlazorServer.Services
{
    public class BlogService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri = "api/Blog";

        public BlogService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BlogApi");
        }

        public async Task<IEnumerable<BlogDto>> GetBlogs()
        {
            var response = await _httpClient.GetAsync(_baseUri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<BlogDto>>();
        }

        public async Task<BlogEditDto> GetBlog(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUri}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BlogEditDto>();
        }

        public async Task<bool> CreateBlog(BlogCreateDto blogCreateDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/blog", blogCreateDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                // Handle any exceptions that might occur.
                return false;
            }
        }

        public async Task UpdateBlog(int id, BlogEditDto blogEditDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUri}/{id}", blogEditDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBlog(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUri}/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            var response = await _httpClient.GetAsync($"api/users/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApplicationUser>();
            }

            return null;
        }
    }
}