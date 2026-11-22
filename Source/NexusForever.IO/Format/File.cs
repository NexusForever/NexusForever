namespace NexusForever.IO.Format
{
    public class File<T> : IFile<T>
    {
        public string Path { get; set; }
        public T Model { get; set; }
    }
}
