using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.World.Chat.Model
{
    public class ChatFormatItemFull : IChatFormatModel
    {
        public ChatFormatType Type => ChatFormatType.ItemFull;
        public ulong ItemGuid { get; set; }
        public uint Item2Id { get; set; }
        public ulong MakerCharacterId { get; set; }
        public ulong CircuitData { get; set; }
        public uint RuneData { get; set; }
        public ulong ThresholdData { get; set; }
        public uint Unknown1 { get; set; }
        public uint WorldRequirements { get; set; }
        public byte Unknown2 { get; set; }
        public uint WorldRequirementItem2Id { get; set; }
        public List<float> ChargeAmounts { get; set; } = new();
        public List<uint> RuneSlotItem2Ids { get; set; } = new();

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            Item2Id = reader.ReadUInt(18u);
            MakerCharacterId = reader.ReadULong();
            CircuitData = reader.ReadULong();
            RuneData = reader.ReadUInt();
            ThresholdData = reader.ReadULong();
            Unknown1 = reader.ReadUInt();
            WorldRequirements = reader.ReadUInt();
            Unknown2 = reader.ReadByte();
            WorldRequirementItem2Id = reader.ReadUInt(18u);

            uint count = reader.ReadUInt(3u);
            for (uint i = 0; i < count; i++)
                ChargeAmounts.Add(reader.ReadSingle());

            count = reader.ReadUInt(4u);
            for (uint i = 0; i < count; i++)
                RuneSlotItem2Ids.Add(reader.ReadUInt());
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(Item2Id, 18u);
            writer.Write(MakerCharacterId);
            writer.Write(CircuitData);
            writer.Write(RuneData);
            writer.Write(ThresholdData);
            writer.Write(Unknown1);
            writer.Write(WorldRequirements);
            writer.Write(Unknown2);
            writer.Write(WorldRequirementItem2Id, 18u);

            writer.Write(ChargeAmounts.Count, 3u);
            ChargeAmounts.ForEach(amount => writer.Write(amount));

            writer.Write(RuneSlotItem2Ids.Count, 4u);
            RuneSlotItem2Ids.ForEach(item2Id => writer.Write(item2Id));
        }
    }
}
