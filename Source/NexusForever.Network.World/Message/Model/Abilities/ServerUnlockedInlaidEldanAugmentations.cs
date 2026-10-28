using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    // The character's unlocked inlaid Eldan augmentations. Unlike ServerAmpList this carries no spec index,
    // the set belongs to the character; the client replaces its array wholesale and caps it at 54 entries.
    [Message(GameMessageOpcode.ServerUnlockedInlaidEldanAugmentations)]
    public class ServerUnlockedInlaidEldanAugmentations : IWritable
    {
        public List<ushort> EldanAugmentations { get; set; } = []; // EldanAugmentationIds

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EldanAugmentations.Count, 7u);
            foreach (ushort eldanAugmentation in EldanAugmentations)
            {
                writer.Write(eldanAugmentation);
            }
        }
    }
}
