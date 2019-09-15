using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRandomRollResponse)]
    public class ServerRandomRollResponse : IWritable
    {

        public ushort realmId { set; get; }
        public ulong characterId { set; get; }
        public int MinRandom { get; set; }
        public int MaxRandom { get; set; }
        public int RandomRollResult { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(realmId, 14u);
            writer.Write(characterId);
            writer.Write(MinRandom);
            writer.Write(MaxRandom);
            writer.Write(RandomRollResult);
        }
    }
}