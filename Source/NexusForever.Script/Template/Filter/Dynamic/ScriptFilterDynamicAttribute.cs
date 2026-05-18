namespace NexusForever.Script.Template.Filter.Dynamic
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptFilterDynamicAttribute<T> : Attribute where T : IScriptFilterDynamic
    {
    }
}
