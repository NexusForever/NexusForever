using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ServerSpellThresholdClear)]
    public class ServerSpellThresholdClear : IWritable
    {
        public uint Spell4Id { get; set; }
        public bool ProcessCooldown { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spell4Id, 18u);
            writer.Write(ProcessCooldown);
        }
    }
}
