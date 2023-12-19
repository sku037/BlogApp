using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Json;
using BlogApp.BlazorServer.Models;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IEnumerable<PostDetailDto>> GetPosts(int blogId)
        {
            var response = await _httpClient.GetAsync($"{_baseUri}/ByBlogId/{blogId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<PostDetailDto>>();
        }

        public async Task<PostDetailDto> GetPost(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUri}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PostDetailDto>();
        }

        public async Task<bool> CreatePost(PostCreateDto postCreateDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUri, postCreateDto);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    // Read the response body as a string if the status code indicates a failure.
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error: " + content);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return false;
            }
        }


        public async Task<bool> UpdatePost(int id, PostEditDto postEditDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUri}/{id}", postEditDto);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    // Read the response body as a string if the status code indicates a failure.
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error: " + content);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return false;
            }
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

        public async Task<IEnumerable<PostDto>> GetPostsByBlogIdWithTags(int blogId)
        {
            var response = await _httpClient.GetAsync($"{_baseUri}/ByBlogWithTags/{blogId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<PostDto>>();
            }
            else
            {
                return new List<PostDto>();
            }
        }

        public async Task<string> SaveImageAsync(byte[] image)
        {
            using var content = new MultipartFormDataContent();
            var imageContent = new ByteArrayContent(image);
            content.Add(imageContent, "file", "upload.jpg");

            var response = await _httpClient.PostAsync($"{_baseUri}/upload", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(); // Return imagePath
            }

            return null; // handle errors
        }
    }
}