using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingAttachUnitToDecorItem)]
    public class ServerHousingAttachUnitToDecorItem : IWritable
    {
        public Identity ResidenceIdentity { get; set; }
        public ulong DecorId { get; set; }
        public ulong AttachedUnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            ResidenceIdentity.Write(writer);
            writer.Write(DecorId);
            writer.Write(AttachedUnitId);
        }
    }
}
