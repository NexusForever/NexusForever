using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPropertyValue : INetworkBuildable<PropertyValue>
    {
        Property Property { get; }
        float BaseValue { get; set; }
        float Value { get; set; }
    }
}