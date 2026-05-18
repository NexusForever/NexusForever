namespace NexusForever.Script.Template.Filter.Dynamic
{
    public interface IScriptFilterDynamic
    {
        /// <summary>
        /// Modify <see cref="IScriptFilterParameters"/> dynamically at runtime.
        /// </summary>
        void Filter(IScriptFilterParameters parameters);
    }
}
