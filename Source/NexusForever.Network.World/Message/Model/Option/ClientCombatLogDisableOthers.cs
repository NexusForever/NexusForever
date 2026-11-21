using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Option
{
    [Message(GameMessageOpcode.ClientCombatLogDisableOthers)]
    public class ClientCombatLogDisableOthers : IReadable
    {
        public bool DisableOtherPlayers { get; set; }

        public void Read(GamePacketReader reader)
        {
            DisableOtherPlayers = reader.ReadUInt() > 0;
        }
    }
}
