using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupSetOrder)]
    public class ClientGroupSetOrder : IReadable
    {
        public ulong GroupId { get; private set; }
        public Identity Member { get; private set; } = new Identity();
        public uint NewIndex { get; private set; }

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            Member.Read(reader);
            NewIndex = reader.ReadUInt();
        }
    }
}