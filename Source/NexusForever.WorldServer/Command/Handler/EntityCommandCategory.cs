using System.Linq;
using System.Text;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.RBAC.Static;
using NexusForever.WorldServer.Game.Reputation;
using NexusForever.WorldServer.Game.Reputation.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Entity, "A collection of commands to modify and query information about an entity.", "entity")]
    [CommandTarget(typeof(WorldEntity))]
    public class EntityCommandCategory : CommandCategory
    {
        [Command(Permission.EntityModify, "A collection of commands to modify an entity.", "modify")]
        public class EntityModifyCommandCategory : CommandCategory
        {
            [Command(Permission.EntityModifyDisplayInfo, "Modify the display info of the target entity.", "displayinfo")]
            public void HandleEntityModifyDisplayInfo(ICommandContext context,
                uint displayInfo)
            {
                if (GameTableManager.Instance.Creature2DisplayInfo.GetEntry(displayInfo) == null)
                {
                    context.SendMessage($"Invalid display info id {displayInfo}!");
                    return;
                }

                context.GetTargetOrInvoker<WorldEntity>().SetDisplayInfo(displayInfo);
            }
        }

        [Command(Permission.EntityInfo, "Get information about the target entity.", "i", "info")]
        public void HandleEntityInfo(ICommandContext context)
        {
            WorldEntity entity = context.GetTargetOrInvoker<WorldEntity>();

            var builder = new StringBuilder();
            BuildHeader(context, builder, entity);

            builder.AppendLine($"XYZ: {entity.Position.X}, {entity.Position.Y}, {entity.Position.Z}");
            builder.AppendLine($"Rotation: {entity.Rotation.X}, {entity.Rotation.Y}, {entity.Rotation.Z}");
            builder.AppendLine($"HP: {entity.Health}/(MAX) | Shield: {entity.Shield}/(MAX)");

            Disposition faction1Disposition = context.Invoker.GetDispositionTo(entity.Faction1);
            Disposition faction2Disposition = context.Invoker.GetDispositionTo(entity.Faction1);
            builder.AppendLine($"Disposition To Me: {entity.Faction1},{faction1Disposition}, {entity.Faction2},{faction2Disposition}");

            context.SendMessage(builder.ToString());
        }

        [Command(Permission.EntityProperties, "Get information about the properties for the target entity.", "p", "properties")]
        public void HandleEntityProperties(ICommandContext context)
        {
            WorldEntity entity = context.GetTargetOrInvoker<WorldEntity>();

            var builder = new StringBuilder();
            BuildHeader(context, builder, entity);

            if (entity.Properties.Count == 0)
                builder.AppendLine("No properties found!");
            else
            {
                foreach ((Property key, PropertyValue value) in entity.Properties.OrderBy(p => p.Key))
                    builder.AppendLine($"{key} - Base: {value.BaseValue} - Value: {value.Value}");
            }

            context.SendMessage(builder.ToString());
        }

        private void BuildHeader(ICommandContext context, StringBuilder builder, WorldEntity target)
        {
            builder.AppendLine("=============================");
            builder.AppendLine($"UnitId: {target.Guid} | DB ID: {target.EntityId} | Type: {target.Type} | CreatureID: {target.CreatureId} | Name: {GetName(target, context.Language)}");
        }

        private string GetName(WorldEntity target, Language language)
        {
            if (target is Player player)
                return player.Name;

            Creature2Entry entry = GameTableManager.Instance.Creature2.GetEntry(target.CreatureId);
            return GameTableManager.Instance.GetTextTable(language).GetEntry(entry.LocalizedTextIdName);
        }
    }
}
