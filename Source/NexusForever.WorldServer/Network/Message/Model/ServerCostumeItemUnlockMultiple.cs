using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCostumeItemUnlockMultiple)]
    public class ServerCostumeItemUnlockMultiple : IWritable
    {
        public CostumeUnlockResult Result { get; set; }
        public List<uint> ItemsIds { get; set; } = new List<uint>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 32u);
            writer.Write(ItemsIds.Count);
            ItemsIds.ForEach(i => writer.Write(i));
        }
    }
}
