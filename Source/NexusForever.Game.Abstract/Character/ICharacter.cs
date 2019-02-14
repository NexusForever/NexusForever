using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Game.Abstract.Character
{
    public interface ICharacter
    {
        ulong CharacterId { get; }
        string Name { get; }
        Sex Sex { get; }
        Race Race { get; }
        Class Class { get; }
        Path Path { get; }
        uint Level { get; }
        Faction Faction1 { get; }
        Faction Faction2 { get; }

        /// <summary>
        /// Returns a <see cref="float"/> representing decimal value, in days, since the character was last online.
        /// </summary>
        float? GetOnlineStatus();
    }
}
