using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Json;
using BlogApp.BlazorServer.Models;
using System.Net;

namespace BlogApp.BlazorServer.Services
{
    public class SearchService
    {
        private readonly HttpClient _httpClient;
        private readonly string _searchUri = "api/Search";

        public SearchService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BlogApi");
        }

        public async Task<IEnumerable<PostDto>> SearchPostsByTag(string tag)
        {
            string encodedTag = WebUtility.UrlEncode(tag);
            var response = await _httpClient.GetAsync($"{_searchUri}/posts-by-tag?tag={encodedTag}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<PostDto>>();
            }
            else
            {
                return new List<PostDto>();
            }
        }

        public async Task<IEnumerable<PostDto>> SearchPostsByUser(string userName)
        {
            var response = await _httpClient.GetAsync($"{_searchUri}/posts-by-user?userName={WebUtility.UrlEncode(userName)}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<PostDto>>();
            }
            else
            {
                return new List<PostDto>();
            }
        }

        public async Task<SearchResultDetailDto> GetSearchResultDetails(string id)
        {
            var response = await _httpClient.GetAsync($"{_searchUri}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SearchResultDetailDto>();
            }
            else
            {
                // Handle errors or return null
                return null;
            }
        }
    }
}