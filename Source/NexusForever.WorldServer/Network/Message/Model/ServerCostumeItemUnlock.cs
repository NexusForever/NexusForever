using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCostumeItemUnlock)]
    public class ServerCostumeItemUnlock : IWritable
    {
        public uint ItemId { get; set; }
        public CostumeUnlockResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemId, 18u);
            writer.Write(Result, 32u);
        }
    }
}
