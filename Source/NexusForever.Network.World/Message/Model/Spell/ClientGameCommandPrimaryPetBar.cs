using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ClientGameCommandPrimaryPetBar)]
    public class ClientGameCommandPrimaryPetBar : IReadable
    {
        public ShortcutSet ShortcutSet { get; private set; }
        public uint Index { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ShortcutSet = reader.ReadEnum<ShortcutSet>(32u);
            Index = reader.ReadUInt();
        }
    }
}
