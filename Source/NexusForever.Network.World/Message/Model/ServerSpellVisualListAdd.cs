using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellVisualListAdd)]
    public class ServerSpellVisualListAdd : IWritable
    {
        public List<ServerSpellVisualAdd> SpellVisualList { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpellVisualList.Count);
            foreach (ServerSpellVisualAdd item in SpellVisualList)
            {
                item.Write(writer);
            }
        }
    }
}
