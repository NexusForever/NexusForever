using System.IO;

namespace NexusForever.Shared.IO
{
    public interface IReadable
    {
        void Read(BinaryReader reader);
    }
}
