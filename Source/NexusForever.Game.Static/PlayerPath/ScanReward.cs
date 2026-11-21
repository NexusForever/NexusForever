namespace NexusForever.Game.Static.PlayerPath
{
    [Flags]
    public enum ScanReward // Meanings a bit speculative. Needs implementation testing to confim.
    {
        RewardForScan        = 0x01,
        RewardForRawScan     = 0x02,
        SpellBuffForScan     = 0x04,
        SpecimenSurveyReward = 0x08,
        HasLoot              = 0x10
    }
}
