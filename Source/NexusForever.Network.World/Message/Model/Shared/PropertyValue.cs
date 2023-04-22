using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared;

public class PropertyValue : IWritable
{
    public Property Property { get; set; }
    public float BaseValue { get; set; }
    public float Value { get; set; }

    public void Write(GamePacketWriter writer)
    {
        writer.Write(Property, 8);
        writer.Write(BaseValue);
        writer.Write(Value);
    }
}
