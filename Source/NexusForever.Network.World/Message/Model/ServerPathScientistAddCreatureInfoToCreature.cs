using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathScientistAddCreatureInfoToCreature)]
    public class ServerPathScientistAddCreatureInfoToCreature : IWritable
    {
        public uint UnitId { get; set; }
        public uint PathScientistCreatureInfoId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(PathScientistCreatureInfoId, 14);
        }
    }
}
