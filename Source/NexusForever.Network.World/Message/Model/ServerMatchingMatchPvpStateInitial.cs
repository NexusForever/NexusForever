using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchPvpStateInitial)]
    public class ServerMatchingMatchPvpStateInitial : IWritable
    {
        public uint Team { get; set; }
        public StateInfo State { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Team, 2u);
            State.Write(writer);
        }
    }
}
