using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellVisualListAdd)]
    public class ServerSpellVisualListAdd : IWritable
    {
        public List<ServerSpellVisualAdd> spellVisualList { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(spellVisualList.Count);
            foreach (ServerSpellVisualAdd item in spellVisualList)
            {
                item.Write(writer);
            }
        }
    }
}
