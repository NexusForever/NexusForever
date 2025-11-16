using NexusForever.Game.Static.Movement;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMovementSpeed)]
    public class ClientMovementSpeed : IReadable
    {
        public MoveSpeed Ticket { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Ticket = reader.ReadEnum<MoveSpeed>(32u);
        }
    }
}
