using Microsoft.AspNetCore.SignalR;

namespace BlogApp.BlazorServer.Services
{
    public class ChatHub : Hub
    {
        private readonly ChatHistoryService _chatHistoryService;

        public ChatHub(ChatHistoryService chatHistoryService)
        {
            _chatHistoryService = chatHistoryService;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            var formattedMessage = $"{DateTime.Now} [{user}]: {message}";
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            await _chatHistoryService.SaveMessageAsync(formattedMessage);
        }
        public async Task ClearChatHistory()
        {
            await _chatHistoryService.ClearHistoryAsync();
            await Clients.All.SendAsync("ChatHistoryCleared");
        }
    }
}