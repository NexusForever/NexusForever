using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    public interface IAppearance : IDatabaseCharacter
    {
        ushort DisplayId { get; set; }
        ItemSlot ItemSlot { get; }
        ulong Owner { get; }
        bool PendingDelete { get; }

        void Delete();
    }
}