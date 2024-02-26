namespace NexusForever.GameTable.Model
{
    public class GuildPerkEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdTitle { get; set; }
        public uint LocalizedTextIdDescription { get; set; }
        public string LuaSprite { get; set; }
        public string LuaName { get; set; }
        public uint PurchaseInfluenceCost { get; set; }
        public uint ActivateInfluenceCost { get; set; }
        public uint Spell4IdActivate { get; set; }
        public uint GuildPerkIdRequired00 { get; set; }
        public uint GuildPerkIdRequired01 { get; set; }
        public uint GuildPerkIdRequired02 { get; set; }
        public uint AchievementIdRequired { get; set; }
    }
}
