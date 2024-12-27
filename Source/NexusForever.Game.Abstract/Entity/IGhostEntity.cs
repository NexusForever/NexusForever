using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Guild;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IGhostEntity : IWorldEntity
    {
        uint OwnerGuid { get; }

        string Name { get; }
        Race Race { get; }
        Class Class { get; }
        Sex Sex { get; }
        List<ulong> GuildIds { get; }
        string GuildName { get; }
        GuildType GuildType { get; }
        ushort Title { get; }

        void Initialise(IPlayer owner);
    }
}
