namespace NexusForever.Script.Finder
{
    public interface IFinder<T>
    {
        /// <summary>
        /// Return a collection of <typeparamref name="T"/>.
        /// </summary>
        List<T> Find();
    }
}