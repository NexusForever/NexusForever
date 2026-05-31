namespace NexusForever.Game.Static.Reward
{
    [Flags]
    public enum GrantedRewardFlags : uint
    {
        GrantItemsOrModifiers = 0x00000001,
        GrantAllTypes         = 0x80000000,

        // Carbine servers seemed to send more flags than these but the client doesn't appear to use them
    }
}
