using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace NexusForever.WorldServer.Command.Contexts
{
    public class WebSocketCommandContext : CommandContext
    {
        public WebSocket Socket { get; }
        public WebSocketCommandContext(WebSocket socket) : base(null)
        {
            Socket = socket;
        }

        public override void SendError(ILogger logger, string text)
        {
            base.SendError(logger, text);
            SendWebSocketMessage(text, "error");
        }

        public override void SendMessage(ILogger logger, string text)
        {
            base.SendMessage(logger, text);
            SendWebSocketMessage(text, "info");
        }

        private void SendWebSocketMessage(string text, string type)
        {
            if (Socket.State == WebSocketState.Open)
            {
                var message = JObject.FromObject(new {text, type}).ToString(Newtonsoft.Json.Formatting.None);
                var messageBytes = Encoding.UTF8.GetBytes(message);
                
                Socket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
                    CancellationToken.None).ContinueWith(
                    async i =>
                    {
                        try
                        {
                            await i.ConfigureAwait(false);
                        }
                        catch
                        {
                            // Ignored.
                        }
                    });
            }
        }
    }
}