using NexusForever.Game.Static.Option;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Option
{
    // Sent when server sends message 0xF1
    // If options are changed during play, they are updated with other messages
    [Message(GameMessageOpcode.ClientCombatOptions)]
    public class ClientCombatOptions : IReadable
    {
        public CastingOptionFlags CastingOptions { get; set; }
        public bool DisableOtherPlayersLogging { get; set; }
        public CombatLogOptions CombatLogDisableFlags { get; set; }

        public void Read(GamePacketReader reader)
        {
            CastingOptions = reader.ReadEnum<CastingOptionFlags>(4u);
            DisableOtherPlayersLogging = reader.ReadBit();
            CombatLogDisableFlags = reader.ReadEnum<CombatLogOptions>(14u);
        }
    }
}
