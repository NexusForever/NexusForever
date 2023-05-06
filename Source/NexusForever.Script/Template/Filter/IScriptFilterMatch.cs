namespace NexusForever.Script.Template.Filter
{
    public interface IScriptFilterMatch
    {
        /// <summary>
        /// Returns if <see cref="IScriptFilterSearch"/> can match with <see cref="IScriptFilterParameters"/>.
        /// </summary>
        bool Match(IScriptFilterSearch search, IScriptFilterParameters parameters);
    }
}
