using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerMountUnlocked, MessageDirection.Server)]
    public class ServerMountUnlocked : IWritable
    {
        public uint SpellId { get; set; }

        public void Write (GamePacketWriter writer)
        {
            writer.Write(SpellId, 18);
        }
    }
}
