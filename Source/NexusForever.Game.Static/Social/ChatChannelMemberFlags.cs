namespace NexusForever.Game.Static.Social
{
    [Flags]
    public enum ChatChannelMemberFlags
    {
        None      = 0x00,
        Owner     = 0x01,
        Moderator = 0x02,
        Muted     = 0x04
    }
}
