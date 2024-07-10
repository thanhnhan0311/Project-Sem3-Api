using arts_core.RequestModels;
using Microsoft.AspNetCore.SignalR;

namespace arts_core.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;
        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public async Task UserJoinRoom(NotificationRequest notificationRequest)
        {
            if (notificationRequest == null)
            {
                _logger.LogError("connection is null.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, notificationRequest.UserId.ToString() +"user");
        }

        public async Task AdminJoinRoom(NotificationRequest notificationRequest)
        {
            if (notificationRequest == null)
            {
                _logger.LogError("connection is null.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, notificationRequest.UserId.ToString() +"admin");
        }


        public async Task SendMessageUser(NotificationRequest notificationRequest)
        {
            if (notificationRequest == null)
            {
                _logger.LogError("ChatConnection is null.");
                return;
            }

            await Clients.Group(notificationRequest.UserId.ToString() + "user").SendAsync("ReceiveMessageUser", notificationRequest.Message);
        }

        public async Task SendMessageAdmin(NotificationRequest notificationRequest)
        {
            if (notificationRequest == null)
            {
                _logger.LogError("ChatConnection is null.");
                return;
            }

            await Clients.Group(notificationRequest.UserId.ToString() + "admin").SendAsync("ReceiveMessageAdmin", notificationRequest.Message);
        }
    }
}
