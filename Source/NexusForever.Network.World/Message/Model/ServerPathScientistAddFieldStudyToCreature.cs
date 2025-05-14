using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathScientistAddFieldStudyToCreature)]
    public class ServerPathScientistAddFieldStudyToCreature : IWritable
    {
        public uint UnitId { get; set; }
        public uint PathScientistFieldStudyId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(PathScientistFieldStudyId, 14);
        }
    }
}
