using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICurrency : IDatabaseCharacter
    {
        CurrencyTypeEntry Entry { get; set; }
        CurrencyType Id { get; }
        ulong CharacterId { get; set; }

        ulong Amount { get; set; }
    }
}