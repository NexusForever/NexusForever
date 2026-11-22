namespace NexusForever.IO.Format
{
    public interface IFileWriter<T>
    {
        void Write(IFile<T> file);
    }
}
