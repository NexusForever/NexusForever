using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMissileActorListAdd)]
    public class ServerMissileActorListAdd : IWritable
    {
        public List<ServerMissileActorAdd> MissileActors { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MissileActors.Count);
            foreach (var actor in MissileActors)
            {
                actor.Write(writer);
            }
        }
    }
}

