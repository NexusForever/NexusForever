using NexusForever.Game.Static.Contact;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerContactsRequestResult)]
    public class ServerContactsRequestResult : IWritable
    {
        public string Unknown0 { get; set; }
        public ContactResult Results { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(Unknown0);
            writer.Write(Results, 6);
        }
    }
}
