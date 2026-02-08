using NexusForever.Game.Static.Entity.Movement;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    // Only sent when the player is knocked down and dashes to break out of it.
    [Message(GameMessageOpcode.ClientCCStateKnockdownBreak)]
    public class ClientCCStateKnockdownBreak : IReadable
    {
        public DashDirection Direction { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Direction = reader.ReadEnum<DashDirection>(3u);
        }
    }
}
