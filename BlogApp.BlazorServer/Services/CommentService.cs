using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Json;
using BlogApp.BlazorServer.Models;

namespace BlogApp.BlazorServer.Services
{
    public class CommentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri = "api/Comment";

        public CommentService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BlogApi");
        }

        public async Task<IEnumerable<CommentDetailDto>> GetCommentsByPostId(int postId)
        {
            var response = await _httpClient.GetAsync($"{_baseUri}/ByPost/{postId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<CommentDetailDto>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // No comments condition
                return new List<CommentDetailDto>();
            }
            else
            {
                throw new HttpRequestException($"Unexpected HTTP response: {response.StatusCode}");
            }
        }


        public async Task<bool> CreateComment(CommentCreateDto commentCreateDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUri, commentCreateDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}