using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model
{
    // If sent while on the CharacterSelect screen, loads into the Game screen
    // If sent while Game screen is already active, reinitializes player data managers.
    [Message(GameMessageOpcode.ServerChangeWorld)]
    public class ServerChangeWorld : IWritable
    {
        public ushort WorldId { get; set; }
        public Position Position { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(WorldId, 15);
            Position.Write(writer);
            writer.Write(Yaw);
            writer.Write(Pitch);
        }
    }
}
