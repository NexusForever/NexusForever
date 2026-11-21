using NexusForever.Game.Static.Costume;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Costume
{
    [Message(GameMessageOpcode.ServerCostumeItemUnlock)]
    public class ServerCostumeItemUnlock : IWritable
    {
        public uint Item2Id { get; set; }
        public CostumeUnlockResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Item2Id, 18u);
            writer.Write(Result, 32u);
        }
    }
}
