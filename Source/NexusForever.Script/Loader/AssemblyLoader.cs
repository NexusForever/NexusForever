namespace NexusForever.Script.Loader
{
    public class AssemblyLoader : IAssemblyLoader
    {
        /// <summary>
        /// Load supplied path into an assembly <see cref="Stream"/>.
        /// </summary>
        public Stream Load(string path)
        {
            return File.Open(path, FileMode.Open, FileAccess.Read);
        }
    }
}
