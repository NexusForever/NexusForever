namespace NexusForever.Game.Static.PlayerPath
{
    [Flags]
    public enum ScanReward // Meanings a bit speculative. Needs implementation testing to confim.
    {
        RewardForScan        = 0x1,
        RewardForRawScan     = 0x2,
        SpellBuffForScan     = 0x4,
        SpecimenSurveyReward = 0x8,
        HasLoot              = 0x10
    }
}
