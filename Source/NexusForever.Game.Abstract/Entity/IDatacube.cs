using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IDatacube : IDatabaseCharacter, INetworkBuildable<Datacube>
    {
        ushort Id { get; }
        DatacubeType Type { get; }
        uint Progress { get; set; }
    }
}