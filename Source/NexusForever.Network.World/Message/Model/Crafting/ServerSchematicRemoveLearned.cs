using NexusForever.Game.Static.Crafting;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Crafting
{
    [Message(GameMessageOpcode.ServerSchematicRemoveLearned)]
    public class ServerSchematicRemoveLearned : IWritable
    {
        public TradeskillType TradeskillId { get; set; }
        public uint TradeskillSchematic2Id { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TradeskillId, 32u);
            writer.Write(TradeskillSchematic2Id);
        }
    }
}
