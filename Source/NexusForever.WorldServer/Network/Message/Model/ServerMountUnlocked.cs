using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerMountUnlocked, MessageDirection.Server)]
    public class ServerMountUnlocked : IWritable
    {
        public uint CreatureId { get; set; }

        /// @TODO : Fix client crash
        public void Write (GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18);
        }
    }
}
