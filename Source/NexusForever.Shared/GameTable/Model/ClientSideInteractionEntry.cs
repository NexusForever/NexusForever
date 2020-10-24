namespace NexusForever.Shared.GameTable.Model
{
    public class ClientSideInteractionEntry
    {
        public uint Id;
        public uint InteractionType;
        public uint Threshold;
        public uint Duration;
        public uint IncrementValue;
        public uint WindowSize;
        public uint Decay;
        public uint Flags;
        public uint TapTime0;
        public uint TapTime1;
        [GameTableFieldArray(4u)]
        public uint[] VisualEffectIds;
        [GameTableFieldArray(5u)]
        public uint[] VisualEffectIdsTarget;
        [GameTableFieldArray(5u)]
        public uint[] VisualEffectIdsCaster;
        public uint LocalizedTextIdContext;
    }
}
