using NexusForever.Network.Message;
using NexusForever.Network;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitStealth)]
    public class ServerUnitStealth : IWritable
    {
        public uint UnitId { get; set; }
        public bool Stealthed { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Stealthed);
        }
    }
}