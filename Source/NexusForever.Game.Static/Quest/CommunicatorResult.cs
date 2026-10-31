namespace NexusForever.Game.Static.Quest
{
    /// <summary>
    /// The first gate a communicator message fails, in the client's order (0x14043d3d0), or <see cref="Eligible"/>.
    /// </summary>
    public enum CommunicatorResult : byte
    {
        Eligible,
        World,
        Zone,
        Class,
        Race,
        Faction,
        Reputation,
        MaxLevel,
        MinLevel,
        QuestCondition,
        Prerequisite,
        Offer
    }
}
