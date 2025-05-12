using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSoliderNewWhackAMoleBurrows)]
    public class ServerPathSoldierNewWhackAMoleBurrows : IWritable
    {
        public uint DelayUntilPop { get; set; }
        public uint[] UnitIds { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(DelayUntilPop);
            writer.Write(UnitIds.Length);
            foreach (var unitId in UnitIds)
            {
                writer.Write(unitId);
            }
        }
    }
}
