using NexusForever.Game.Static.Event;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPublicEventStart)]
    public class ServerPublicEventStart : IWritable
    {
        public uint PublicEventId { get; set; }
        public PublicEventTeam PublicEventTeamId { get; set; }
        public uint ElapsedTimeMs { get; set; }
        public bool Busy { get; set; }
        public List<PublicEventObjective> Objectives { get; set; } = [];
        public List<uint> Locations { get; set; } = [];
        public uint RewardType { get; set; }
        public uint[] RewardThreshold { get; set; } = new uint[3];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PublicEventId, 14);
            writer.Write(PublicEventTeamId, 14);
            writer.Write(ElapsedTimeMs);
            writer.Write(Busy);

            writer.Write(Objectives.Count);
            foreach (PublicEventObjective objective in Objectives)
                objective.Write(writer);

            writer.Write(Locations.Count);
            foreach (uint location in Locations)
                writer.Write(location);

            writer.Write(RewardType);

            foreach (uint threshold in RewardThreshold)
                writer.Write(threshold);

            writer.Write(0, 14);
        }
    }
}
