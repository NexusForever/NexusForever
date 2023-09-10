using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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
