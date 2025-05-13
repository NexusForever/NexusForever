using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellList)]
    public class ServerSpellList : IWritable
    {
        public List<SpellInit> SpellInitData { get; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpellInitData.Count);
            foreach (var data in SpellInitData)
            {
                data.Write(writer);
            }
        }
    }
}

