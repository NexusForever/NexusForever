using NexusForever.Database;
using NexusForever.Database.Auth;

namespace NexusForever.Game.Abstract.Account.Costume
{
    public interface ICostumeUnlock : IDatabaseAuth, IDatabaseState
    {
        uint ItemId { get; }
    }
}