using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerContactsList)]
    public class ServerContactsList : IWritable
    {
        public List<ContactData> Contacts { get; set; } = new List<ContactData>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Contacts.Count, 16u);
            Contacts.ForEach(f => f.Write(writer));
        }
    }
}
