namespace NexusForever.Game.Abstract
{
    public interface IIdentity
    {
        ushort RealmId { get; set; }
        ulong Id { get; set; }
    }
}
