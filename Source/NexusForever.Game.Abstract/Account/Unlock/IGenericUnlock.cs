using NexusForever.Database.Auth;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Account.Unlock
{
    public interface IGenericUnlock : IDatabaseAuth
    {
        GenericUnlockEntryEntry Entry { get; }
        GenericUnlockType Type { get; }
    }
}