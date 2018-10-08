using System.IO;

namespace NexusForever.Shared.Network.Message
{
    public interface IWritable
    {
        void Write(GamePacketWriter writer);
    }
}
