using NexusForever.Game.Static.Option;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Option
{
    // Sent whenever one of the options is changed
    [Message(GameMessageOpcode.ClientCombatLogDisables)]
    public class ClientCombatLogDisables : IReadable
    {
        public CombatLogOptions DisableFlags { get; set; }

        public void Read(GamePacketReader reader)
        {
            DisableFlags = reader.ReadEnum<CombatLogOptions>(32u);
        }
    }
}
