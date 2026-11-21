using NexusForever.Database.Character;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPathEntry : IDatabaseCharacter
    {
        Static.PlayerPath.Path Path { get; set; }
        ulong CharacterId { get; set; }
        bool Unlocked { get; set; }
        uint TotalXp { get; set; }
        byte LevelRewarded { get; set; }
    }
}