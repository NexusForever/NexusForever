using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IAppearance : IDatabaseCharacter
    {
        ulong Owner { get; }
        ItemSlot ItemSlot { get; }
        ushort DisplayId { get; set; }

        bool PendingDelete { get; }

        void Delete();
    }
}