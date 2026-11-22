using NexusForever.Shared;

namespace NexusForever.IO.Format
{
    public class FileBinaryReader<T> : IFileReader<T> where T : IBinaryReadable
    {
        #region Dependency Injection

        private readonly IFactory<IFile<T>> fileFactory;

        public FileBinaryReader(
            IFactory<IFile<T>> fileFactory)
        {
            this.fileFactory = fileFactory;
        }

        #endregion

        public virtual IFile<T> Read(string path)
        {
            using Stream stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream);

            IFile<T> file = fileFactory.Resolve();
            file.Path = path;
            file.Model.Read(reader);

            return file;
        }
    }
}
