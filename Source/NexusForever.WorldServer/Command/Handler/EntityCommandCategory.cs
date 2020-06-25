using System.Linq;
using System.Text;
using NexusForever.Database.World.Model;
using NexusForever.Shared.Database;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.RBAC.Static;
using NLog;

namespace NexusForever.WorldServer.Command.Handler
{

    [Command(Permission.Entity, "A collection of commands to modify and query information about an entity.", "entity")]
    [CommandTarget(typeof(WorldEntity))]
    public class EntityCommandCategory : CommandCategory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        [Command(Permission.Entity, "Add an Entity by Creature Id", "heal")]
        public void HandleEntityHeal(ICommandContext context)
        {
            
        }

        [Command(Permission.Entity, "Add an Entity by Creature Id", "add")]
        public void HandleEntityAdd(ICommandContext context,
            uint unitId)
        {
            var creatureEntry = GameTableManager.Instance.Creature2.GetEntry(unitId);

            if (creatureEntry == null)
            {
                context.SendMessage($"Invalid creature id {unitId}!");
                return;
            }

            var npcModel = new EntityModel()
            {
                Id = DatabaseManager.Instance.WorldDatabase.GetNewEntityId(),
                Creature = unitId,
                Area = (ushort)context.GetTargetOrInvoker<Player>().Zone.Id,
                DisplayInfo = creatureEntry.Id,
                OutfitInfo = (ushort)creatureEntry.Creature2OutfitGroupId,
                X = context.Invoker.Position.X,
                Y = context.Invoker.Position.Y,
                Z = context.Invoker.Position.Z,
                Rx = context.Invoker.Rotation.X,
                Ry = context.Invoker.Rotation.Y,
                Rz = context.Invoker.Rotation.Z,
                World = (ushort)context.Invoker.Map.Entry.Id,
                Faction1 = (ushort)creatureEntry.FactionId,
                Faction2 = (ushort)creatureEntry.FactionId,
                Type = (byte)creatureEntry.CreationTypeEnum
            };

            npcModel.Id = DatabaseManager.Instance.WorldDatabase.GetNewEntityId();
            
            var entity = new NonPlayer();
            entity.Initialise(npcModel, context.Invoker.Position, context.Invoker.Rotation);
            context.GetTargetOrInvoker<Player>().Map.EnqueueAdd(entity, context.Invoker.Position);

            // Add New Entity to the EntityCache
            var cache = EntityCacheManager.Instance.GetEntityCache((ushort)context.Invoker.Map.Entry.Id);
            cache.AddEntity(npcModel);
            var activeNPCGrid = context.Invoker.Map.GetGrid(context.Invoker.Position);
            var l = cache.GetEntities(activeNPCGrid.Coord.X, activeNPCGrid.Coord.Z).ToList();

            // Make sure we have the entity information stored
            DatabaseManager.Instance.WorldDatabase.SaveEntities(l);
                        
        }

        [Command(Permission.Entity, "Delete an Entity by Target", "del")]
        public void HandleEntityDelete(ICommandContext context)
        {
            
            if (context.Target == null)
            {
                context.SendMessage($"Invalid target!");
                return;
            }

            if (context.Target is Player player)
            {
                context.SendMessage($"You cannot delete a player...");
                return;
            }
            
            context.Target.RemoveFromMap();
            var model = DatabaseManager.Instance.WorldDatabase
                .GetEntities((ushort)context.Invoker.Map.Entry.Id)
                .Where(x => x.Id == context.Target.EntityId)
                .FirstOrDefault();
            EntityCacheManager.Instance.GetEntityCache((ushort)context.Invoker.Map.Entry.Id).DelEntity(model);
            DatabaseManager.Instance.WorldDatabase.RemoveEntity(model);


        }

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
            builder.AppendLine($"HP: {entity.Health}/(MAX) | Shield: {entity.Shield}/(MAX)");

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
