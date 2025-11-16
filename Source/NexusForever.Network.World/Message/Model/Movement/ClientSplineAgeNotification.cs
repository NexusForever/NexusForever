using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Movement
{
    // Notifies the server when a live spline has been on the client every 15 minutes
    // The server can decide to leave the spline there or remove it if no longer needed
    [Message(GameMessageOpcode.ClientSplineAgeNotification)]
    public class ClientSplineAgeNotification : IReadable
    {
        public uint Spline2Id { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Spline2Id = reader.ReadUInt();
        }
    }
}
