using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCostumeSave)]
    public class ServerCostumeSave : IWritable
    {
        public int Index { get; set; }
        public byte MannequinIndex { get; set; }
        public CostumeSaveResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Index);
            writer.Write(MannequinIndex, 2u);
            writer.Write(Result, 4u);
        }
    }
}
