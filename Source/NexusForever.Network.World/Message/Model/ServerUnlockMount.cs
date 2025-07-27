using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Triggers the MountUnlocked ui event and creates a chat log message
    [Message(GameMessageOpcode.ServerUnlockMountNotification)]
    public class ServerUnlockMount : IWritable
    {
        public uint Spell4Id { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spell4Id, 18u);
        }
    }
}

