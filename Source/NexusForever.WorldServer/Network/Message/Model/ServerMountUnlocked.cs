using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerMountUnlocked, MessageDirection.Server)]
    public class ServerMountUnlocked : IWritable
    {
        public uint SpellID { get; set; }
        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpellID, 18);
        }
    }
}