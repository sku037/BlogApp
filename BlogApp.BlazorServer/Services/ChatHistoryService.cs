using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Json;
using BlogApp.BlazorServer.Models;
using Microsoft.Extensions.Http;
using System.IO;

namespace BlogApp.BlazorServer.Services
{
    public class ChatHistoryService
    {
        private readonly string _filePath;

        public ChatHistoryService(string filePath)
        {
            _filePath = filePath;
        }

        public async Task SaveMessageAsync(string message)
        {
            await File.AppendAllTextAsync(_filePath, $"{message}\n");
        }

        public async Task<IEnumerable<string>> LoadMessagesAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<string>();
            }

            var lines = await File.ReadAllLinesAsync(_filePath);
            return lines;
        }
        public async Task ClearHistoryAsync()
        {
            if (File.Exists(_filePath))
            {
                await File.WriteAllTextAsync(_filePath, string.Empty);
            }
        }
    }

}