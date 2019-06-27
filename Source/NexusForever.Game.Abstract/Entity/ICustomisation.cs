using NexusForever.Database.Character;

namespace NexusForever.Game.Entity
{
    public interface ICustomisation : IDatabaseCharacter
    {
        ulong CharacterId { get; }
        uint Label { get; }
        bool PendingDelete { get; }
        uint Value { get; set; }

        void Delete();
    }
}