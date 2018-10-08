using System.IO;

namespace NexusForever.Shared.Network.Message
{
    public interface IReadable
    {
        void Read(GamePacketReader reader);
    }
}
