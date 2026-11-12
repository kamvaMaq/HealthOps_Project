using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

namespace HealthOps_Project.Hubs
{
    public class NotificationHub : Hub
    {
        // Track connected users and their roles
        private static readonly ConcurrentDictionary<string, UserInfo> _connectedUsers = new();

        public async Task SubscribeToScriptManagerGroup(string wardName)
        {
            // Optional grouping by ward
            await Groups.AddToGroupAsync(Context.ConnectionId, "ScriptManagers");

            // Also add to ward-specific group if wardName is provided
            if (!string.IsNullOrEmpty(wardName))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Ward_{wardName}");
            }
        }

        public async Task SubscribeToNotifications(string userId, string userRole, string wardName = null)
        {
            // Store user information
            _connectedUsers[Context.ConnectionId] = new UserInfo
            {
                UserId = userId,
                Role = userRole,
                WardName = wardName,
                ConnectionId = Context.ConnectionId
            };

            // Add to role-based group
            await Groups.AddToGroupAsync(Context.ConnectionId, userRole);

            // Add to ward-specific group if applicable
            if (!string.IsNullOrEmpty(wardName))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Ward_{wardName}");
            }

            // Script managers get additional group
            if (userRole == "ScriptManager" || userRole == "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "ScriptManagers");
            }
        }

        public async Task SendNotification(string message, string notificationType = "Info")
        {
            // Send to all connected clients
            await Clients.All.SendAsync("ReceiveNotification", message, notificationType);
        }

        public async Task SendToRole(string role, string message, string notificationType = "Info")
        {
            // Send to specific role group
            await Clients.Group(role).SendAsync("ReceiveNotification", message, notificationType);
        }

        public async Task SendToWard(string wardName, string message, string notificationType = "Info")
        {
            // Send to specific ward group
            await Clients.Group($"Ward_{wardName}").SendAsync("ReceiveNotification", message, notificationType);
        }

        public async Task SendToUser(string userId, string message, string notificationType = "Info")
        {
            // Find the connection ID for the user
            var userConnection = _connectedUsers.Values.FirstOrDefault(u => u.UserId == userId);
            if (userConnection != null)
            {
                await Clients.Client(userConnection.ConnectionId).SendAsync("ReceiveNotification", message, notificationType);
            }
        }

        public async Task SendScriptUpdateNotification(string wardName, string scriptId, string action)
        {
            var message = $"Script {scriptId} has been {action} in ward {wardName}";
            await Clients.Group("ScriptManagers").SendAsync("ReceiveScriptUpdate", message, scriptId, action, wardName);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Remove user from tracking
            _connectedUsers.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }

        // Helper method to get connected users count (for monitoring)
        public int GetConnectedUsersCount()
        {
            return _connectedUsers.Count;
        }

        // Helper method to get users by role
        public int GetUsersCountByRole(string role)
        {
            return _connectedUsers.Values.Count(u => u.Role == role);
        }
    }

    public class UserInfo
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        public string WardName { get; set; }
        public string ConnectionId { get; set; }
    }
}