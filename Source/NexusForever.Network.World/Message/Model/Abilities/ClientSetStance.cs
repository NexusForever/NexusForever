using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    // Sent when client uses lua function GameLib::SetCurrentClassInnateAbilityIndex or /setstance command.
    [Message(GameMessageOpcode.ClientSetStance)]
    public class ClientSetStance : IReadable
    {
        public byte InnateIndex { get;  private set; }

        public void Read(GamePacketReader reader)
        {
            InnateIndex = reader.ReadByte(8u);
        }
    }
}
