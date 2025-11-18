using NexusForever.Network.Message;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ServerMissileActorRemove)]
    public class ServerMissileActorRemove : IWritable
    {
        public uint MissileActorId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MissileActorId);
        }
    }
}
