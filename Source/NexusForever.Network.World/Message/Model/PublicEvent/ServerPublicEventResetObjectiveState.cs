using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    // Finds all objectives on the PublicEvent not associated with the provided PublicEventTeamId and resets the objective's state
    [Message(GameMessageOpcode.ServerPublicEventResetObjectiveState)]
    public class ServerPublicEventResetObjectiveState : IWritable
    {
        public uint PublicEventId { get; set; }
        public uint PublicEventTeamId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PublicEventId, 14u);
            writer.Write(PublicEventTeamId, 14u);
        }
    }
}
