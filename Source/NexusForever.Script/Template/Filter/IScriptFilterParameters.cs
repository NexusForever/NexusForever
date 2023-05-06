namespace NexusForever.Script.Template.Filter
{
    public interface IScriptFilterParameters
    {
        Type ScriptType { get; }
        HashSet<uint> Id { get; set; }
        HashSet<uint> CreatureId { get; set; }

        void Initialise(Type type);
    }
}
