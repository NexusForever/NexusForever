namespace NexusForever.Script.Template.Filter
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptFilterActivePropIdAttribute : Attribute
    {
        public ulong[] ActivePropId { get; }

        public ScriptFilterActivePropIdAttribute(params ulong[] activePropId)
        {
            ActivePropId = activePropId;
        }
    }
}
