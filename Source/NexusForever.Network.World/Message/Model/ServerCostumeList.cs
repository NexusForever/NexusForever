using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCostumeList)]
    public class ServerCostumeList : IWritable
    {
        public List<Costume> Costumes { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Costumes.Count);
            Costumes.ForEach(c => c.Write(writer));
        }
    }
}
