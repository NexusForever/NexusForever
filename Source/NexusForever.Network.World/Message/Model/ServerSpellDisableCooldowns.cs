using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Resets all cooldowns and gives spells infinite charges
    [Message(GameMessageOpcode.ServerSpellDisableCooldowns)]
    public class ServerSpellDisableCooldowns : IWritable
    {
        public bool DisableCooldowns { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(DisableCooldowns);
        }
    }
}
