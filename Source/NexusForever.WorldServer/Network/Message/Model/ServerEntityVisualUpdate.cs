using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityVisualUpdate)]
    public class ServerEntityVisualUpdate : IWritable
    {
        public uint UnitId { get; set; }
        public byte Race { get; set; }
        public byte Sex { get; set; }
        public uint CreatureId { get; set; }
        public uint DisplayInfo { get; set; }
        public ushort OutfitInfo { get; set; }
        public uint ItemColorSetId { get; set; }
        public bool Unknown6 { get; set; }

        public List<ItemVisual> ItemVisuals { get; set; } = new List<ItemVisual>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Race, 5u);
            writer.Write(Sex, 2u);
            writer.Write(CreatureId, 18u);
            writer.Write(DisplayInfo, 17u);
            writer.Write(OutfitInfo, 15u);
            writer.Write(ItemColorSetId);
            writer.Write(Unknown6);
            writer.Write(ItemVisuals.Count);
            ItemVisuals.ForEach(u => u.Write(writer));
        }
    }
}
