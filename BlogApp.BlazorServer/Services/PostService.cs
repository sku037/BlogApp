using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Json;
using BlogApp.BlazorServer.Models;

namespace BlogApp.BlazorServer.Services
{
    public class PostService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri = "api/Post";

        public PostService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BlogApi");
        }

        public async Task<IEnumerable<Post>> GetPosts()
        {
            var response = await _httpClient.GetAsync(_baseUri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<Post>>();
        }

        public async Task<Post> GetPost(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUri}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Post>();
        }

        public async Task<bool> CreatePost(PostCreateDto postCreateDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUri, postCreateDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task UpdatePost(int id, PostEditDto postEditDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUri}/{id}", postEditDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeletePost(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUri}/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<Post>> GetPostsByBlogId(int blogId)
        {
            var response = await _httpClient.GetAsync($"{_baseUri}/ByBlog/{blogId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<Post>>();
        }
    }
}