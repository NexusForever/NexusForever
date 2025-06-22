namespace NexusForever.Game.Static.Pvp
{
    // The forfeit reasons are all treated the same on the client even though the live game sent specific forfeit reasons.
    public enum DuelFinishReason
    {
        Defeated = 0x0,
        Forfeited1 = 0x1, // might be from leaving duel area
        Forfeited2 = 0x2, // result from ClientDuelForfeit
        Forfeited3 = 0x3,
        DeclinedRequest = 0x4,
        DuelCancelled = 0x5,
    }
}
