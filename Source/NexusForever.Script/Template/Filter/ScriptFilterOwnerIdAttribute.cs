namespace NexusForever.Script.Template.Filter
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptFilterOwnerIdAttribute : Attribute
    {
        public uint[] Id { get; }

        public ScriptFilterOwnerIdAttribute(params uint[] id)
        {
            Id = id;
        }
    }

}
