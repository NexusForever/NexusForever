using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Sets unit to have its interaction status re-evaluated.
    // This is typically triggered by the handler for a relevant message so not sure when this would be used.
    [Message(GameMessageOpcode.ServerSetUnitAsDirty)]
    public class ServerSetUnitAsDirty : IWritable
    {
        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
        }
    }
}
