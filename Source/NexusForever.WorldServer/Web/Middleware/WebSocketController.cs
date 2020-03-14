using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Web.Middleware
{
    public class WebSocketMiddleware
    {
        public WebSocketMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        public RequestDelegate Next { get; }

        private class ClientMessage
        {
            [JsonProperty("message")]
            public string Message { get; set; }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws/commands")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    while (webSocket.CloseStatus == null)
                    {
                        (WebSocketReceiveResult result, ClientMessage clientMessage) = await ReceiveObjectAsync<ClientMessage>(webSocket, context.RequestAborted);
                        if (result.CloseStatus != null)
                            continue;
                        await CommandManager.Instance.HandleCommandAsync(new WebSocketCommandContext(webSocket), clientMessage.Message, false).ConfigureAwait(false);
                    }
                }
                else
                    context.Response.StatusCode = 400;
            }
            else
                await Next(context);
        }

        async Task<(WebSocketReceiveResult result, T obj)> ReceiveObjectAsync<T>(WebSocket socket,
            CancellationToken cancellationToken)
        {
            (WebSocketReceiveResult result, string data) result = await ReceiveStringAsync(socket, cancellationToken);
            if (result.result.CloseStatus != null)
                return (result.result, default);

            return (result.result, JsonConvert.DeserializeObject<T>(result.data));
        }

        async Task<(WebSocketReceiveResult result, string data)> ReceiveStringAsync(WebSocket socket,
            CancellationToken cancellationToken)
        {
            (WebSocketReceiveResult result, IEnumerable<byte> data) messageResult = await ReceiveFullMessageAsync(socket, cancellationToken);
            if (messageResult.result.CloseStatus != null)
                return (messageResult.result, null);

            string message = Encoding.UTF8.GetString(messageResult.data.ToArray());
            return (messageResult.result, message);
        }

        async Task<(WebSocketReceiveResult result, IEnumerable<byte> data)> ReceiveFullMessageAsync(
            WebSocket socket, CancellationToken cancellationToken)
        {
            WebSocketReceiveResult response;
            var message = new List<byte>();

            var buffer = new byte[4096];
            do
            {
                response = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                message.AddRange(new ArraySegment<byte>(buffer, 0, response.Count));
            } while (!response.EndOfMessage);

            return (response, message);
        }
    }
}
