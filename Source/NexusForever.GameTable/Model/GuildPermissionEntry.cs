namespace NexusForever.GameTable.Model
{
    public class GuildPermissionEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdDescription { get; set; }
        public string LuaVariable { get; set; }
        public uint LocalizedTextIdCommand { get; set; }
        public uint GuildTypeEnumFlags { get; set; }
        public uint DisplayIndex { get; set; }
    }
}
