using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Entity")]
    public class EntityCommandHandler : CommandCategory
    {
        public EntityCommandHandler()
            : base(true, "entity")
        {
        }

        private WorldEntity GetTarget(CommandContext context)
        {
            if (context.Session.Player.TargetGuid == 0u)
            {
                context.SendErrorAsync("You must select a target.");
                return null;
            }

            WorldEntity targetEntity = context.Session.Player.Map.GetEntity<WorldEntity>(context.Session.Player.TargetGuid);
            if (targetEntity == null)
            {
                context.SendErrorAsync("No target was found. Make sure your target is in range.");
                return null;
            }

            return targetEntity;
        }

        private string GetCreatureName(WorldEntity targetEntity, Language lang)
        {
            if (!(targetEntity is Player player))
            {
                Creature2Entry entry = GameTableManager.Instance.Creature2.GetEntry(targetEntity.CreatureId);
                TextTable tt = GameTableManager.Instance.GetTextTable(lang);
                return tt.GetEntry(entry.LocalizedTextIdName);
            }

            return player.Name;
        }

        private string GetDBIdString(WorldEntity targetEntity)
        {
            if (targetEntity.EntityId > 0)
                return $"| DB ID: {targetEntity.EntityId}";
            else
                return "";
        }

        private StringBuilder GetOutput(string command, WorldEntity targetEntity, Language lang)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("=============================");
            stringBuilder.AppendLine($"{GetCreatureName(targetEntity, lang)} | UnitId: {targetEntity.Guid} | CreatureID: {targetEntity.CreatureId} | Type: {targetEntity.Type}");

            return stringBuilder;
        }

        [SubCommandHandler("info", "Get Information about the target entity.")]
        [SubCommandHandler("i", "Get Information about the target entity.")]
        public Task EntityInfoCommand(CommandContext context, string command, string[] parameters)
        {
            WorldEntity targetEntity = GetTarget(context);
            if (targetEntity == null)
                return Task.CompletedTask;

            StringBuilder output = GetOutput(command, targetEntity, context.Language);

            output.AppendLine($"XYZ: {targetEntity.Position.X}, {targetEntity.Position.Y}, {targetEntity.Position.Z} {GetDBIdString(targetEntity)}");
            output.AppendLine($"HP: {targetEntity.Health}/(MAX) | Shield: {targetEntity.Shield}/(MAX)");

            context.SendMessageAsync(output.ToString()).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        [SubCommandHandler("properties", "")]
        [SubCommandHandler("p", "")]
        public Task EntityPropertiesCommand(CommandContext context, string command, string[] parameters)
        {
            WorldEntity targetEntity = GetTarget(context);
            if (targetEntity == null)
                return Task.CompletedTask;

            StringBuilder output = GetOutput(command, targetEntity, context.Language);

            if (targetEntity.Properties.Count == 0)
                output.AppendLine($"No properties found!");
            else
            {
                foreach (KeyValuePair<Property, PropertyValue> property in targetEntity.Properties.OrderBy(p => p.Key))
                    output.AppendLine($"{property.Key} - Base: {property.Value.BaseValue} - Value: {property.Value.Value}");
            }

            context.SendMessageAsync(output.ToString()).ConfigureAwait(false);

            return Task.CompletedTask;
        }
    }
}
