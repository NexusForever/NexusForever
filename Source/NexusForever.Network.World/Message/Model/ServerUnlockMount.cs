using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerUnlockMount)]
    public class ServerUnlockMount : IWritable
    {
        public uint Spell4Id { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spell4Id, 18u);
        }
    }
}

