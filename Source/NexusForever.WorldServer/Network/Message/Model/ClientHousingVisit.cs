using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingVisit)]
    public class ClientHousingVisit : IReadable
    {
        public ulong Unknown0 { get; private set; }
        public TargetPlayerIdentity Unknown8 { get; } = new TargetPlayerIdentity();
        public string PlayerName { get; private set; }
        public TargetPlayerIdentity Unknown20 { get; } = new TargetPlayerIdentity();
        public string Unknown30 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unknown0   = reader.ReadULong();
            Unknown8.Read(reader);
            PlayerName = reader.ReadWideString();
            Unknown20.Read(reader);
            Unknown30  = reader.ReadWideString();
        }
    }
}
