namespace NexusForever.IO.Format
{
    public class FileTextWriter<T> : IFileWriter<T> where T : ITextWritable
    {
        public void Write(IFile<T> file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file.Path));

            using Stream stream = File.Create(file.Path);
            using var reader = new StreamWriter(stream);
            file.Model.Write(reader);
        }
    }
}
