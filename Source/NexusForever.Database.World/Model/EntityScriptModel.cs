namespace NexusForever.Database.World.Model
{
    public class EntityScriptModel
    {
        public uint Id { get; set; }
        public string ScriptName { get; set; }

        public EntityModel Entity { get; set; }
    }
}
