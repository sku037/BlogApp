using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Json;
using BlogApp.BlazorServer.Models;

namespace BlogApp.BlazorServer.Services
{
    public class TagService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri = "api/Tag";

        public TagService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BlogApi");
        }

        public async Task<IEnumerable<TagDto>> GetAllTags()
        {
            var response = await _httpClient.GetAsync(_baseUri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<TagDto>>();
        }

        public async Task<TagDto> GetTag(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUri}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TagDto>();
        }

        public async Task<bool> CreateTag(TagCreateDto tagCreateDto)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUri, tagCreateDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTag(int id, TagDto tagDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUri}/{id}", tagDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTag(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUri}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}