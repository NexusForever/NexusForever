namespace NexusForever.Script.Template.Filter
{
    public class ScriptFilterSearch : IScriptFilterSearch
    {
        public Type ScriptType { get; private set; }
        public uint? Id { get; private set; }
        public uint? CreatureId { get; private set; }
        public uint? TargetGroupId { get; private set; }

        public IScriptFilterSearch FilterByScriptType<T>() where T : IScript
        {
            ScriptType = typeof(T);
            return this;
        }

        public IScriptFilterSearch FilterById(uint id)
        {
            Id = id;
            return this;
        }

        public IScriptFilterSearch FilterByCreatureId(uint id)
        {
            CreatureId = id;
            return this;
        }

        public IScriptFilterSearch FilterByTargetGroupId(uint id)
        {
            TargetGroupId = id;
            return this;
        }
    }
}
