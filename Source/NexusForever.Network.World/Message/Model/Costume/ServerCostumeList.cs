using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Costume
{
    // Sends all the saved costumes to the client, both personal and mannequin costumes.
    // Max 12 personal costumes and 5 mannequin costumes. Indexes higher than that are ignored by the client.
    [Message(GameMessageOpcode.ServerCostumeList)]
    public class ServerCostumeList : IWritable
    {
        public List<Costume> Costumes { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Costumes.Count);
            Costumes.ForEach(c => c.Write(writer));
        }
    }
}
