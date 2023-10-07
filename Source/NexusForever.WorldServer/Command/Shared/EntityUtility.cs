using System.Text;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.WorldServer.Command.Shared
{
    public static class EntityUtility
    {
        public static void BuildHeader(StringBuilder builder, IWorldEntity target, Language language)
        {
            builder.AppendLine("=============================");
            builder.AppendLine($"UnitId: {target.Guid} | DB ID: {target.EntityId} | Type: {target.Type} | CreatureID: {target.CreatureId} | Name: {GetName(target, language)}");
        }

        public static string GetName(IWorldEntity target, Language language)
        {
            if (target is IPlayer player)
                return player.Name;

            Creature2Entry entry = GameTableManager.Instance.Creature2.GetEntry(target.CreatureId);
            return GameTableManager.Instance.GetTextTable(language).GetEntry(entry.LocalizedTextIdName) ?? "Unknown";
        }
    }
}
