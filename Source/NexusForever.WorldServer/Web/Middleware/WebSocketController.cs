using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Web.Controllers
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
                        (WebSocketReceiveResult, ClientMessage) message = await ReceiveObjectAsync<ClientMessage>(webSocket, context.RequestAborted);
                        if(message.Item1.CloseStatus != null) continue;
                        await CommandManager.HandleCommandAsync(new WebSocketCommandContext(webSocket), message.Item2.Message, false).ConfigureAwait(false);
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await Next(context);
            }
        }

        async Task<(WebSocketReceiveResult, T)> ReceiveObjectAsync<T>(WebSocket socket,
            CancellationToken cancellationToken)
        {
            (WebSocketReceiveResult, string) result = await ReceiveStringAsync(socket, cancellationToken);
            if (result.Item1.CloseStatus != null)
            {
                return (result.Item1, default(T));
            }

            return (result.Item1, JToken.Parse(result.Item2).ToObject<T>());
        }

        async Task<(WebSocketReceiveResult, string)> ReceiveStringAsync(WebSocket socket,
            CancellationToken cancellationToken)
        {
            WebSocketReceiveResult result; string message;
            (WebSocketReceiveResult result, IEnumerable<byte> data) messageResult = await ReceiveFullMessageAsync(socket, cancellationToken);
            result = messageResult.result;
            if (result.CloseStatus != null)
            {
                return (result, null);
            }

            message = Encoding.UTF8.GetString(messageResult.data.ToArray());



            return (result, message);
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
