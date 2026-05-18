namespace NexusForever.Game.Abstract.Guild
{
    public interface IGuildInvite
    {
        ulong GuildId { get; set; }
        ulong InviteeId { get; set; }
    }
}