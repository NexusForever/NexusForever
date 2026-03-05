using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextItemFullFormat : IChatChannelTextFormatModel
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
    }
}
