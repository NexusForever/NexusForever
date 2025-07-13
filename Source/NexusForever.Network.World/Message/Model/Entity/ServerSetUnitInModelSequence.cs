using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSetUnitInModelSequence)]
    public class ServerSetUnitInModelSequence : IWritable
    {
        public uint UnitId { get; set; }
        public uint ModelSequenceId { get; set; }
        public float StartTime { get; set; } // not used
        public float Speed { get; set; } // not used 
        public uint Layer { get; set; } 
        public ushort Seed { get; set; } // used to randomize the model sequence

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(ModelSequenceId);
            writer.Write(StartTime);
            writer.Write(Speed);
            writer.Write(Layer);
            writer.Write(Seed);   
        }
    }
}
