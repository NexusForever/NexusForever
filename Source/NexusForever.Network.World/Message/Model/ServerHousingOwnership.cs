using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingOwnership)]
    public class ServerHousingOwnership : IWritable
    {
        public TargetResidence TargetResidence { get; set; }
        public uint Value { get; set; } 

        public void Write(GamePacketWriter writer)
        {
            TargetResidence.Write(writer);
            writer.Write(Value);
        }
    }
}
