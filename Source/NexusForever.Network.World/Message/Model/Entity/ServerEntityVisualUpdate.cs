using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityVisualUpdate)]
    public class ServerEntityVisualUpdate : IWritable
    {
        public uint UnitId { get; set; }
        public byte RaceId { get; set; }
        public byte GenderId { get; set; }
        public uint Creature2Id { get; set; }
        public uint DisplayInfoId { get; set; }
        public ushort DisplayOutfitInfoId { get; set; }
        public float Scale { get; set; } // not used by client
        public bool Unknown6 { get; set; } // TBC, do not display race and gender as well as 

        public List<ItemVisual> ItemVisuals { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(RaceId, 5u);
            writer.Write(GenderId, 2u);
            writer.Write(Creature2Id, 18u);
            writer.Write(DisplayInfoId, 17u);
            writer.Write(DisplayOutfitInfoId, 15u);
            writer.Write(Scale);
            writer.Write(Unknown6);
            writer.Write(ItemVisuals.Count);
            ItemVisuals.ForEach(u => u.Write(writer));
        }
    }
}
