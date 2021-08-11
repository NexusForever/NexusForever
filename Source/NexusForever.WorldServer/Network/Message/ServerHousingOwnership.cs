using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message
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
