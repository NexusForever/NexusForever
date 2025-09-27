using NexusForever.Network.Message;
using NetworkMessage = NexusForever.Network.Message.Model.Shared.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    // Client only processes message when on RealmSelect screen.
    [Message(GameMessageOpcode.ServerRealmList)]
    public class ServerRealmList : IWritable
    {
        public ulong Unknown0 { get; set; }
        public List<RealmInfo> Realms { get; set; } = [];
        public List<NetworkMessage> Messages { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(Realms.Count);
            Realms.ForEach(s => s.Write(writer));
            writer.Write(Messages.Count);
            Messages.ForEach(s => s.Write(writer));
        }
    }
}
