namespace NexusForever.IO.Format
{
    public interface IFileReader<T>
    {
        IFile<T> Read(string path);
    }
}
