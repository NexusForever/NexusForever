using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    // Triggers SettlerInfrastructureStarted lua event
    [Message(GameMessageOpcode.ServerPathSettlerInfrastructureStarted)]
    public class ServerPathSettlerInfrastructureStarted : IWritable
    {
        public uint Unused { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unused);
        }
    }
}
