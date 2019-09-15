using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRandomRollResponse)]
    public class ServerRandomRollResponse : IWritable
    {
        public ushort RealmId { set; get; }
        public ulong CharacterId { set; get; }
        public int MinRandom { get; set; }
        public int MaxRandom { get; set; }
        public int RandomRollResult { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(CharacterId);
            writer.Write(MinRandom);
            writer.Write(MaxRandom);
            writer.Write(RandomRollResult);
        }
    }
}