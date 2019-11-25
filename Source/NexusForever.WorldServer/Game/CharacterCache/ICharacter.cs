using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.CharacterCache
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

        float GetOnlineStatus();
    }
}
