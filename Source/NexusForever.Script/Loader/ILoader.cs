namespace NexusForever.Script.Loader
{
    public interface ILoader
    {
        /// <summary>
        /// Load supplied path into an assembly <see cref="Stream"/>.
        /// </summary>
        Stream Load(string path);
    }
}
