using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellUpdate)]
    public class ServerSpellUpdate : IWritable
    {
        public uint Spell4BaseId { get; set; }
        public byte TierIndex { get; set; }
        public byte SpecIndex { get; set; }
        public bool Activated { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spell4BaseId, 18u);
            writer.Write(TierIndex, 4u);
            writer.Write(SpecIndex, 3u);
            writer.Write(Activated);
        }
    }
}
