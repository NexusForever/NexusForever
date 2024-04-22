using NexusForever.Database.Character;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IBone : IDatabaseCharacter
    {
        ulong Owner { get; }
        byte BoneIndex { get; }
        float BoneValue { get; set; }
        
        bool PendingDelete { get; }

        void Delete();
    }
}