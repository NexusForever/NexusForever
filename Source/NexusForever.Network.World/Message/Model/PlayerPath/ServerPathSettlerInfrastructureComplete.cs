using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    // Triggers SettlerInfrastructureComplete lua event
    [Message(GameMessageOpcode.ServerPathSettlerInfrastructureComplete)]
    public class ServerPathSettlerInfrastructureComplete : IWritable
    {
        public uint Unused { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unused);
        }
    }
}
