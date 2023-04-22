using System.Linq;
using System.Text;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Entity, "A collection of commands to modify and query information about an entity.", "entity")]
    [CommandTarget(typeof(IWorldEntity))]
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

                context.GetTargetOrInvoker<IWorldEntity>().SetDisplayInfo(displayInfo);
            }
        }

        [Command(Permission.EntityInfo, "Get information about the target entity.", "i", "info")]
        public void HandleEntityInfo(ICommandContext context)
        {
            IWorldEntity entity = context.GetTargetOrInvoker<IWorldEntity>();

            var builder = new StringBuilder();
            BuildHeader(context, builder, entity);

            builder.AppendLine($"XYZ: {entity.Position.X}, {entity.Position.Y}, {entity.Position.Z}");
            builder.AppendLine($"HP: {entity.Health}/(MAX) | Shield: {entity.Shield}/(MAX)");

            Disposition faction1Disposition = context.Invoker.GetDispositionTo(entity.Faction1);
            Disposition faction2Disposition = context.Invoker.GetDispositionTo(entity.Faction1);
            builder.AppendLine($"Disposition To Me: {entity.Faction1},{faction1Disposition}, {entity.Faction2},{faction2Disposition}");

            context.SendMessage(builder.ToString());
        }

        [Command(Permission.EntityProperties, "Get information about the properties for the target entity.", "p", "properties")]
        public void HandleEntityProperties(ICommandContext context)
        {
            IWorldEntity entity = context.GetTargetOrInvoker<IWorldEntity>();

            var builder = new StringBuilder();
            BuildHeader(context, builder, entity);

            if (entity.Properties.Count == 0)
                builder.AppendLine("No properties found!");
            else
            {
                foreach ((Property key, IPropertyValue value) in entity.Properties.OrderBy(p => p.Key))
                    builder.AppendLine($"{key} - Base: {value.BaseValue} - Value: {value.Value}");
            }

            context.SendMessage(builder.ToString());
        }

        private void BuildHeader(ICommandContext context, StringBuilder builder, IWorldEntity target)
        {
            builder.AppendLine("=============================");
            builder.AppendLine($"UnitId: {target.Guid} | DB ID: {target.EntityId} | Type: {target.Type} | CreatureID: {target.CreatureId} | Name: {GetName(target, context.Language)}");
        }

        private string GetName(IWorldEntity target, Language language)
        {
            if (target is IPlayer player)
                return player.Name;

            Creature2Entry entry = GameTableManager.Instance.Creature2.GetEntry(target.CreatureId);
            return GameTableManager.Instance.GetTextTable(language).GetEntry(entry.LocalizedTextIdName);
        }
    }
}
