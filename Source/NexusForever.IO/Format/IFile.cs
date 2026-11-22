namespace NexusForever.IO.Format
{
    public interface IFile<T>
    {
        string Path { get; set; }
        public T Model { get; set; }
    }
}
