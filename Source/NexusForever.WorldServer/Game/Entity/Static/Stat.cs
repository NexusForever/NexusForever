namespace NexusForever.WorldServer.Game.Entity.Static
{
    public enum Stat
    {
        Health              = 0,
        Dash                = 9,
        Level               = 10,
        MentorLevel         = 11,
        State               = 12, // 0 = Standing (Combat Pose), 1 = Sitting, 2 = Laying Down, 3 = Standing (Idle)
        Sheathed            = 15, // 0 = Unsheathed Weapons, 1 = Sheathed Weapons
        Shield              = 20,
        LowHpAndGhostEffect = 32, // Need confirmation what screen effects are happening
    }
}
