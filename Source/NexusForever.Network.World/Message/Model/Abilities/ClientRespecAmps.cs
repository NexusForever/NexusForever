using NexusForever.Game.Static.Abilities;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    [Message(GameMessageOpcode.ClientRespecAmps)]
    public class ClientRespecAmps : IReadable
    {
        public byte SpecIndex { get; private set; }
        public AmpRespecType RespecType { get; private set; }
        public uint Value { get; private set; } // Is EldanAugmentationId for RespecType = Single
                                                // Is EldanAugmentationCategoryId for RespecType = Section
                                                // Is zero for RespecType = Full

        public void Read(GamePacketReader reader)
        {
            SpecIndex = reader.ReadByte(3u);
            RespecType = reader.ReadEnum<AmpRespecType>(3u);
            Value = reader.ReadUInt();
        }
    }
}
