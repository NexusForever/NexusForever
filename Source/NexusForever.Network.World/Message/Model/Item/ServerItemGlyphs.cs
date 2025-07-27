using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemGlyphs)]
    public class ServerItemGlyphs : IWritable
    {
        public ulong ItemGuid { get; set; }
        public uint RandomGlyphData { get; set; }
        public uint[] GlyphItem2Ids { get; set; } = new uint[8];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(RandomGlyphData);
            writer.Write(GlyphItem2Ids.Length, 4u);
            foreach (uint glyphItem2Id in GlyphItem2Ids)
            {
                writer.Write(glyphItem2Id);
            }
        }
    }
}
