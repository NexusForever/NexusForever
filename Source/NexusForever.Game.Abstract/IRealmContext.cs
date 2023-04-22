namespace NexusForever.Game.Abstract
{
    public interface IRealmContext
    {
        ushort RealmId { get; }
        string Motd { get; set; }

        void Initialise();

        /// <summary>
        /// Get the current Server Time in FileTime format.
        /// </summary>
        ulong GetServerTime();
    }
}