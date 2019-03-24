using System.IO;

namespace NexusForever.Shared.IO
{
    public interface IWritable
    {
        void Write(BinaryWriter writer);
    }
}
