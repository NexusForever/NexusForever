namespace NexusForever.Game.Static.Quest
{
    public enum TargetGroupType
    {
        CreatureIdGroup     = 1, // This is the majority of entries in TargetGroups TBL
        CreatureIdGroup2    = 2, // Unsure how this is used, possibly an exclusion list
        FactionIdGroup      = 3,
        NotFactionIdGroup   = 4, // This is used to say "Don't target this faction"
        PlayerRaceIdGroup   = 5,
        Unknown7            = 7,
        Unknown9            = 9,
        OtherTargetGroup    = 10, // This is used to target other TargetGroup(s) data
        OtherTargetGroup2   = 11, // Target other TargetGroup(s) - Used in kill quest.
        CreatureRaceIdGroup = 12, // This targets the Creature2Entry.RaceId
        Unknown13           = 13
    }
}
