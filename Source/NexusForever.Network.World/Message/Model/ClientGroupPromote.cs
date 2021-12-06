using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupPromote)]
    public class ClientGroupPromote : IReadable
    {
        public ulong GroupId { get; set; }

        public TargetPlayerIdentity TargetedPlayer { get; set; } = new TargetPlayerIdentity();

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            TargetedPlayer.Read(reader);
        }
    }
}
