using System.Collections.Generic;
using System.Linq;
using System.Text;
using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Shared;

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
                if (displayInfo != 0 && GameTableManager.Instance.Creature2DisplayInfo.GetEntry(displayInfo) == null)
                {
                    context.SendMessage($"Invalid display info id {displayInfo}!");
                    return;
                }

                context.GetTargetOrInvoker<IWorldEntity>().DisplayInfo = displayInfo;
            }
        }

        [Command(Permission.EntityInfo, "Get information about the target entity.", "i", "info")]
        public void HandleEntityInfo(ICommandContext context)
        {
            IWorldEntity entity = context.GetTargetOrInvoker<IWorldEntity>();

            var builder = new StringBuilder();
            EntityUtility.BuildHeader(builder, entity, context.Language);

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
            EntityUtility.BuildHeader(builder, entity, context.Language);

            var properties = entity.GetProperties().ToList();
            if (properties.Count == 0)
                builder.AppendLine("No properties found!");
            else
            {
                foreach (IPropertyValue value in properties.OrderBy(p => p.Property))
                    builder.AppendLine($"{value.Property} - Base: {value.BaseValue} - Value: {value.Value}");
            }

            context.SendMessage(builder.ToString());
        }

        [Command(Permission.EntityThreat, "A collection of commands to modify threat for this entity.", "threat")]
        public class EntityThreatCommandCategory : EntityCommandCategory
        {
            [Command(Permission.EntityThreatAdjust, "Adjust threat between the target and yourself.", "a", "adjust")]
            public void HandleEntityThreatAdjust(ICommandContext context,
                [Parameter("Amount to adjust by. This can be a negative or positive number.")]
                int threat)
            {
                IWorldEntity entity = context.GetTargetOrInvoker<IWorldEntity>();
                if (entity == context.Invoker)
                {
                    context.SendMessage($"You must have a target other than yourself.");
                    return;
                }

                if (!(entity is IUnitEntity unit))
                {
                    context.SendMessage($"Only entities of a Unit type can have threat.");
                    return;
                }

                unit.ThreatManager.UpdateThreat(context.Invoker as IUnitEntity, threat);
            }

            [Command(Permission.EntityThreatList, "Prints the target's threat list.", "l", "list")]
            public void HandleEntityThreatList(ICommandContext context)
            {
                IWorldEntity entity = context.GetTargetOrInvoker<IWorldEntity>();

                if (!(entity is IUnitEntity unit))
                {
                    context.SendMessage($"Only entities of a Unit type can have threat.");
                    return;
                }

                var builder = new StringBuilder();
                EntityUtility.BuildHeader(builder, unit, context.Language);
                builder.AppendLine("=============================");
                builder.AppendLine("# | GUID | Name | Threat");
                builder.AppendLine("-----------------------------");

                int i = 1;
                List<IHostileEntity> hostiles = unit.ThreatManager.ToList();
                if (hostiles.Count == 0u)
                    builder.AppendLine("No threat targets.");

                foreach (IHostileEntity hostile in hostiles)
                {
                    IUnitEntity hostileEntity = entity.GetVisible<IUnitEntity>(hostile.HatedUnitId);
                    builder.AppendLine($"{i++} | ({hostileEntity.Guid}) | {EntityUtility.GetName(hostileEntity, Language.English)} | {hostile.Threat}");
                }

                context.SendMessage(builder.ToString());
            }

            [Command(Permission.EntityThreatClear, "Clears the target's threat list.", "c", "clear")]
            public void HandleEntityThreatClear(ICommandContext context)
            {
                IWorldEntity entity = context.GetTargetOrInvoker<IWorldEntity>();

                if (!(entity is IUnitEntity unit))
                {
                    context.SendMessage($"Only entities of a Unit type can have threat.");
                    return;
                }

                unit.ThreatManager.ClearThreatList();
            }

            [Command(Permission.EntityThreatRemove, "Remove an entity from the target's threat list.", "r", "remove")]
            public void HandleEntityThreatRemove(ICommandContext context,
                [Parameter("UnitId of the Entity to remove from the Target. Ommitting this defaults to you being the target.")]
                uint? unitId)
            {
                IWorldEntity entity = context.GetTargetOrInvoker<IWorldEntity>();

                if (!(entity is IUnitEntity unit))
                {
                    context.SendMessage($"Only entities of a Unit type can have threat.");
                    return;
                }

                unitId ??= context.Invoker.Guid;

                if (unit.Guid == unitId)
                {
                    if (unit.Guid == context.Invoker.Guid)
                        context.SendMessage($"You must have a target other than yourself.");
                    else
                        context.SendMessage($"You cannot remove a target from its own threat list.");
                    return;
                }

                unit.ThreatManager.RemoveHostile((uint)unitId);
            }
        }
    }
}
