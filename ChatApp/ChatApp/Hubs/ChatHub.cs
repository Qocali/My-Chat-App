using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public class ChatHub:Hub
    {
        private readonly string _BotUser;
        private readonly IDictionary<string, UserConnection> _connections;
        public ChatHub(IDictionary<string, UserConnection> connections)
        {
            _BotUser = "My ChatApp";
            _connections = connections;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
               _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _BotUser, $"{userConnection.User} has left");
                SendConnectedUsers(userConnection.Room);
            }
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId,out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage",userConnection.User,message);
            }
        }
            public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);
            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage",_BotUser,$"{userConnection.User} has joined {userConnection.Room}");
            _connections[Context.ConnectionId] = userConnection;
            await SendConnectedUsers(userConnection.Room);
        }
        public  Task SendConnectedUsers(string room)
        {
            var users=_connections.Values.Where(c=>c.Room==room).Select(c=>c.User);
            return  Clients.Group(room).SendAsync("UserInRoom",users);
        }
    }
}
