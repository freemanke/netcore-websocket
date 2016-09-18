using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace Server
{
    public class ChatMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly Encoding Encoder = Encoding.UTF8;


        public ChatMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ChatMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var ws = await context.WebSockets.AcceptWebSocketAsync();
                await HandlerAsync(ws);
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private async Task HandlerAsync(WebSocket ws)
        {
            var token = CancellationToken.None;
            var buffer = new ArraySegment<byte>(new byte[4096]);

            while (ws.State == WebSocketState.Open)
            {
                WebSocketReceiveResult received = null;
                try { received = await ws.ReceiveAsync(buffer, token); }
                catch (Exception e)
                {
                    _logger.LogError($"Client '{ws.GetHashCode()}' unusally disconnected (ReceiveAsync()), exception: '{e.Message}'");
                    break;
                }
                switch (received.MessageType)
                {
                    case WebSocketMessageType.Text:
                        try
                        {
                            var sendBuffer = buffer.Array.Take(received.Count).ToArray();
                            await ws.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, token);
                            _logger.LogInformation($"Sent to {ws.GetHashCode()}: {Encoding.UTF8.GetString(sendBuffer)}");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Client '{ws.GetHashCode()}' unusally disconnected (SendAsync()), exception: '{e.Message}'");
                            break;
                        }

                        break;
                    case WebSocketMessageType.Binary:
                        _logger.LogError($"Chat middleware dismisses any binary data.");
                        break;
                    case WebSocketMessageType.Close:
                        _logger.LogInformation($"Client '{ws.GetHashCode()}' disconnected");
                        await ws.CloseAsync(received.CloseStatus.Value, received.CloseStatusDescription, token);
                        break;
                }
            }
        }
    }

    public static class ChatMiddlewareExtensions
    {
        public static IApplicationBuilder UseChatMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ChatMiddleware>();
        }
    }
}
