using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class Cooldown : IWritable
    {
        public byte Type { get; set; }
        public uint SpellId { get; set; }
        public uint TypeId { get; set; }
        public uint TimeRemaining { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 3u);
            writer.Write(SpellId, 18u);
            writer.Write(TypeId);
            writer.Write(TimeRemaining);
        }
    }
}
