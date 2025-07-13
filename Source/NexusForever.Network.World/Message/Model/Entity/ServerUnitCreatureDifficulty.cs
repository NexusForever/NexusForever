using NexusForever.Network.Message;
using NexusForever.Network;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitCreatureDifficulty)]
    public class ServerUnitCreatureDifficulty : IWritable
    {
        public uint UnitId { get; set; }
        public ushort Creature2DifficultyId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Creature2DifficultyId);
        }
    }
}