namespace NexusForever.Game.Static.Rewards
{
    [Flags]
    public enum GrantedRewardFlags : uint
    {
        Item     = 0x00000001,
        Modifier = 0x00000001,
        Essence  = 0x80000000,

        // Carbine servers seemed to send more flags than these but the client doesn't appear to use them
    }
}
