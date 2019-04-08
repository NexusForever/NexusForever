using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
