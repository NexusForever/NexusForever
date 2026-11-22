namespace NexusForever.IO.Format
{
    public class FileBinaryWriter<T> : IFileWriter<T> where T : IBinaryWritable
    {
        public void Write(IFile<T> file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file.Path));

            using Stream stream = File.Create(file.Path);
            using var reader = new BinaryWriter(stream);
            file.Model.Write(reader);
        }
    }
}
