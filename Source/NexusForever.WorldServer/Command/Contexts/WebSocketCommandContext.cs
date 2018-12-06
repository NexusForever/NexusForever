using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NexusForever.WorldServer.Command.Contexts
{
    public class WebSocketCommandContext : CommandContext
    {
        public WebSocket Socket { get; }

        public WebSocketCommandContext(WebSocket socket)
            : base(null)
        {
            Socket = socket;
        }

        public override async Task SendErrorAsync(string text)
        {
            await base.SendErrorAsync(text);
            await SendWebSocketMessage(text, "error");
        }

        public override async Task SendMessageAsync( string text)
        {
            await base.SendMessageAsync(text);
            await SendWebSocketMessage(text, "info");
        }

        private async Task SendWebSocketMessage(string text, string type)
        {
            if (Socket.State == WebSocketState.Open)
            {
                string message = JObject.FromObject(new {text, type}).ToString(Formatting.None);
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                try
                {
                    await Socket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
                        CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to send {0} message to websocket client, message was: {1}",
                        type, text);
                }
            }
        }
    }
}
