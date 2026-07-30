namespace NexusForever.Game.Static.Item
{
    [Flags]
    public enum ItemFlags
    {
        Soulbound        = 0x01,
        Unknown02        = 0x02,
        Unknown04        = 0x04,
        NotReturnable    = 0x08,
        NotSalvageable   = 0x10,
        VendorWontBuy    = 0x20,
        AccountTradeable = 0x40,
        NotTradeable     = 0x80,
    }
}
