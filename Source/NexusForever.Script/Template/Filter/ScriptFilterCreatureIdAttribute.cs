namespace NexusForever.Script.Template.Filter
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptFilterCreatureIdAttribute : Attribute
    {
        public uint[] CreatureId { get; }

        public ScriptFilterCreatureIdAttribute(params uint[] creatureId)
        {
            CreatureId = creatureId;
        }
    }
}
