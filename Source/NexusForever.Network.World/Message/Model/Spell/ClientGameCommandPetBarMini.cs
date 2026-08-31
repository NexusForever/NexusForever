using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ClientGameCommandPetBarMini)]
    public class ClientGameCommandPetBarMini : IReadable
    {
        public ShortcutSet ShortcutSet { get; set; }
        public uint Index { get; set; }

        public void Read(GamePacketReader reader)
        {
            ShortcutSet = reader.ReadEnum<ShortcutSet>(32u);
            Index = reader.ReadUInt();
        }
    }
}
