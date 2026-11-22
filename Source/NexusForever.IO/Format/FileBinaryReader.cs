using NexusForever.Shared;

namespace NexusForever.IO.Format
{
    public class FileBinaryReader<T> : IFileReader<T> where T : class, IBinaryReadable
    {
        #region Dependency Injection

        private readonly IFactory<T> modelFactory;
        private readonly IFactory<IFile<T>> fileFactory;

        public FileBinaryReader(
            IFactory<T> modelFactory,
            IFactory<IFile<T>> fileFactory)
        {
            this.modelFactory = modelFactory;
            this.fileFactory  = fileFactory;
        }

        #endregion

        public virtual IFile<T> Read(string path)
        {
            using Stream stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream);

            T model = modelFactory.Resolve();
            IFile<T> file = fileFactory.Resolve();
            file.Path = path;
            file.Model = model;
            file.Model.Read(reader);

            return file;
        }
    }
}
