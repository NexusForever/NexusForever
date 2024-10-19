namespace NexusForever.Script.Template.Filter
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptFilterScriptNameAttribute : Attribute
    {
        public string ScriptName { get; }

        public ScriptFilterScriptNameAttribute(string scriptName)
        {
            ScriptName = scriptName;
        }
    }
}
