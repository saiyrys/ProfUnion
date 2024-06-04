using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Profunion.Models.UserModels;

namespace Profunion.Services.MessagesServices
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private ConcurrentDictionary<string, string> _connectionUserMap = new ConcurrentDictionary<string, string>();

        public WebSocketManagerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                string conncetionID = Guid.NewGuid().ToString();
                _sockets.TryAdd(conncetionID, webSocket);
                await HandleWebSocket(conncetionID, webSocket);
            }
        }
        private async Task HandleWebSocket(string connectionId, WebSocket webSocket)
        {
            byte[] buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await SendMessageToAllAsync($"Connection ID {connectionId} sent: {message}");

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            WebSocket socket;
            _sockets.TryRemove(connectionId, out socket);
            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        private async Task SendMessageAsync(string senderConnectionId, string message)
        {
            string[] parts = message.Split(':');
            if (parts.Length != 2)
                return;

            string recipientId = parts[0];
            string messageText = parts[1];

            // Находим соединение получателя и отправляем сообщение
            foreach (var (connectionId, userId) in _connectionUserMap)
            {
                if (userId == recipientId && _sockets.TryGetValue(connectionId, out WebSocket recipientWebSocket) && recipientWebSocket.State == WebSocketState.Open)
                {
                    await recipientWebSocket.SendAsync(Encoding.UTF8.GetBytes(messageText), WebSocketMessageType.Text, true, CancellationToken.None);
                    return;
                }
            }
        }

        private bool IsValidRole(User sender, User recipient)
        {
            if (sender.role == "USER" && recipient.role == "ADMIN")
            {
                return true;
            }

            if (sender.role == "ADMIN" && sender.role == "ADMIN")
            {
                return true;
            }

            return false;
        }

        private async Task SendMessageToAllAsync(string message)
        {
            foreach (var socket in _sockets)
            {
                if (socket.Value.State == WebSocketState.Open)
                {
                    await socket.Value.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }

    public static class WebSocketExtensions
    {
        public static IApplicationBuilder UseWebSocketManager(this IApplicationBuilder app)
        {
            return app.UseMiddleware<WebSocketManagerMiddleware>();
        }
    }
}

