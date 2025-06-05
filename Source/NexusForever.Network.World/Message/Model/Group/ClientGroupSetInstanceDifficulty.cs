using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupSetInstanceDifficulty)]
    public class ClientGroupSetInstanceDifficulty : IReadable
    {
        public ulong GroupId { get; private set; }
        public InstanceDifficulty Difficulty { get; private set; }

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            Difficulty = reader.ReadEnum<InstanceDifficulty>(2);
        }
    }
}
