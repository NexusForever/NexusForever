using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity
{
    /// <summary>
    /// Client acknowledges <see cref="ServerDialogStart"/>. Empty NPE dialogs never send choices,
    /// so without an end packet the UI sits on "Uploading…" forever.
    /// </summary>
    public class ClientDialogOpenedHandler : IMessageHandler<IWorldSession, ClientDialogOpened>
    {
        public void HandleMessage(IWorldSession session, ClientDialogOpened dialogOpened)
        {
            session.EnqueueMessageEncrypted(new ServerDialogEnd());
        }
    }
}
