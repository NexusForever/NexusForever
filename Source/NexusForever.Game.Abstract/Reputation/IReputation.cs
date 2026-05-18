using NexusForever.Database.Character;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Abstract.Reputation
{
    public interface IReputation : IDatabaseCharacter
    {
        IFactionNode Entry { get; }
        Faction Id { get; }
        float Amount { get; set; }
    }
}