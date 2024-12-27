using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPublicEventObjectiveUpdate)]
    public class ServerPublicEventObjectiveUpdate : IWritable
    {
        public PublicEventObjective Objective { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Objective.Write(writer);
        }
    }
}
