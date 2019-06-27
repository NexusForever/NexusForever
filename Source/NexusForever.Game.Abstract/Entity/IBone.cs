using NexusForever.Database.Character;

namespace NexusForever.Game.Entity
{
    public interface IBone : IDatabaseCharacter
    {
        byte BoneIndex { get; }
        float BoneValue { get; set; }
        ulong Owner { get; }
        bool PendingDelete { get; }

        void Delete();
    }
}