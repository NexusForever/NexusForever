using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared;

public class StatValueUpdate : IWritable
{
    public Stat Stat { get; set; }
    public StatType Type { get; set; }
    public float Value { get; set; }

    public void Write(GamePacketWriter writer)
    {
        writer.Write(Stat, 5u);

        switch (Type)
        {
            case StatType.Integer:
                writer.Write((uint)Value);
                break;
            case StatType.Float:
                writer.Write(Value);
                break;
        }
    }
}
