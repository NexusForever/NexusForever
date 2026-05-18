using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    [Message(GameMessageOpcode.ServerAmpPowerUpdate)]
    public class ServerAmpPowerUpdate : IWritable
    {
        public ushort BonusPower { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BonusPower);
        }
    }
}
