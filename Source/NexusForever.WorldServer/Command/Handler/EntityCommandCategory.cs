using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        /// <summary>
        /// Outputs a json File in the .exe directory which contains a grouped list of mobs
        /// </summary>
        [Command(Permission.Entity, "Listing Creature2 Entities - npctool | display | model", "list")]
        public void HandleEntityList(ICommandContext context, string filter)
        {
            // TODO: Convert to Sub-Commands

            // Generates an NPC Template List
            if (filter.ToLower() == "template")
            {
                var creatureData = GameTableManager.Instance.Creature2.Entries;
                List<NPCSpawnModel> CreatureList = new List<NPCSpawnModel>();
                List<EntityModel> dbEntities = new List<EntityModel>();

                var WorldList = DatabaseManager.Instance.WorldDatabase.GetUniqueWorlds();
                foreach(ushort worldid in WorldList)
                {
                    dbEntities.AddRange(DatabaseManager.Instance.WorldDatabase.GetEntities(worldid));
                }

                List<string> EnteredNames = new List<string>();
                
                foreach(var entity in dbEntities)
                {

                    string entityName = creatureData.Where(e => e.Id == entity.Creature).Select(x => x.Description).FirstOrDefault();
                    if(EnteredNames.Count > 0)
                    {
                        if (EnteredNames.Contains(entityName)) continue;
                    }
                        
                    EnteredNames.Add(entityName);

                    // Link the Creature2 Entry Description with an existing world entity
                    CreatureList.Add(new NPCSpawnModel { 
                        Creature = entity.Creature,
                        Description = entityName,
                        Type = entity.Type,
                        activePropId = entity.ActivePropId,
                        displayInfo = entity.DisplayInfo,
                        OutfitInfo = entity.OutfitInfo,
                        faction1 = entity.Faction1,
                        faction2 = entity.Faction2
                    });
                }
                

                File.WriteAllText("NPCTemplates.json", JsonConvert.SerializeObject(CreatureList));
            }

            // Create creatures_ByDisplayGroupId.json, Creatures Grouped by DisplayGroupId (file size in excess of 180mb)
            if (filter.ToLower() == "display")
            {
                var creatureData = GameTableManager.Instance.Creature2.Entries
                    .GroupBy(o => o.Creature2DisplayGroupId)
                    .ToDictionary(g => g.Key, g=> g.ToList());
                File.WriteAllText("creatures_ByDisplayGroupId.json", JsonConvert.SerializeObject(creatureData));
                return;
            }

            // Create creatures_ByModelInfoId.json, Creatures Grouped by ModelInfoId (file size in excess of 180mb)
            if (filter.ToLower() == "model")
            {
                var creatureData = GameTableManager.Instance.Creature2.Entries
                    .GroupBy(o => o.Creature2ModelInfoId)
                    .ToDictionary(g => g.Key, g => g.ToList());
                File.WriteAllText("creatures_ByModelInfoId.json", JsonConvert.SerializeObject(creatureData));
                return;
            }
        }

        // Temporary Disable: Adjusting The Way It Adds NPCs
        [Command(Permission.Entity, "Add an Entity by Creature Id", "add")]
        public void HandleEntityAdd(ICommandContext context,
            [Parameter("X coordinate for target teleport position.")]
            string creatureId,
            [Parameter("X coordinate for target teleport position.")]
            string creatureType,
            [Parameter("X coordinate for target teleport position.")]
            string displayInfo,
            [Parameter("X coordinate for target teleport position.")]
            string outfitInfo,
            [Parameter("X coordinate for target teleport position.")]
            string faction1,
            [Parameter("X coordinate for target teleport position.")]
            string faction2,
            [Parameter("X coordinate for target teleport position.")]
            string activePropId)
        {
            if(string.IsNullOrEmpty(activePropId))
            {
                context.SendError("Invalid property.");
                return;
            }

            if (string.IsNullOrEmpty(faction2))
            {
                context.SendError("Invalid property.");
                return;
            }

            if (string.IsNullOrEmpty(faction1))
            {
                context.SendError("Invalid property.");
                return;
            }
            if (string.IsNullOrEmpty(outfitInfo))
            {
                context.SendError("Invalid property.");
                return;
            }
            if (string.IsNullOrEmpty(displayInfo))
            {
                context.SendError("Invalid property.");
                return;
            }
            if (string.IsNullOrEmpty(creatureType))
            {
                context.SendError("Invalid property.");
                return;
            }
            if (string.IsNullOrEmpty(creatureId))
            {
                context.SendError("Invalid property.");
                return;
            }

            var creatureEntry = GameTableManager.Instance.Creature2.GetEntry(ulong.Parse(creatureId));

            if (creatureEntry == null)
            {
                context.SendMessage($"Invalid creature id!");
                return;
            }

            SpawnObject(context, creatureId, creatureType, displayInfo, outfitInfo, faction1, faction2, activePropId);
        }

        private void SpawnObject(ICommandContext context,
            string creatureId,
            string creatureType,
            string displayInfo,
            string outfitInfo,
            string faction1,
            string faction2,
            string activePropId)
        {
            var npcModel = new EntityModel()
            {
                Id = DatabaseManager.Instance.WorldDatabase.GetNewEntityId(),
                Creature = uint.Parse(creatureId),
                Area = (ushort)context.Invoker.Zone.Id,
                DisplayInfo = uint.Parse(displayInfo),
                OutfitInfo = ushort.Parse(outfitInfo),
                X = context.Invoker.Position.X,
                Y = context.Invoker.Position.Y,
                Z = context.Invoker.Position.Z,
                Rx = context.Invoker.Rotation.X,
                Ry = context.Invoker.Rotation.Y,
                Rz = context.Invoker.Rotation.Z,
                World = (ushort)context.Invoker.Map.Entry.Id,
                Faction1 = ushort.Parse(faction1),
                Faction2 = ushort.Parse(faction2),
                Type = byte.Parse(creatureType)
            };

            var entity = new NonPlayer();
            entity.CreateFlags = EntityCreateFlag.SpawnAnimation;
            entity.Initialise(npcModel, context.Invoker.Position, context.Invoker.Rotation);
            // Cannot use "context.GetTargetOrInvoker()" because if you are targeting an NPC it returns the NPC
            context.Invoker.Map.EnqueueAdd(entity, context.Invoker.Position);
            entity.CalculateProperties();

            // Add New Entity to the EntityCache
            //var cache = EntityCacheManager.Instance.GetEntityCache((ushort)context.Invoker.Map.Entry.Id);
            //cache.AddEntity(npcModel);

            #region Old Spawn Code
            //var creatureModels = GameTableManager.Instance.Creature2DisplayInfo.GetEntry(creatureEntry.Creature2DisplayGroupId);

            //var npcModel = new EntityModel()
            //{
            //    Id = DatabaseManager.Instance.WorldDatabase.GetNewEntityId(),
            //    Creature = unitId,
            //    Area = (ushort)context.Invoker.Zone.Id,
            //    DisplayInfo = creatureModels.ModelTextureId00,
            //    OutfitInfo = (ushort)creatureEntry.Creature2OutfitGroupId,
            //    X = context.Invoker.Position.X,
            //    Y = context.Invoker.Position.Y,
            //    Z = context.Invoker.Position.Z,
            //    Rx = context.Invoker.Rotation.X,
            //    Ry = context.Invoker.Rotation.Y,
            //    Rz = context.Invoker.Rotation.Z,
            //    World = (ushort)context.Invoker.Map.Entry.Id,
            //    Faction1 = (ushort)creatureEntry.FactionId,
            //    Faction2 = (ushort)creatureEntry.FactionId,
            //    Type = (byte)creatureEntry.CreationTypeEnum
            //};

            //npcModel.Id = DatabaseManager.Instance.WorldDatabase.GetNewEntityId();

            //var entity = new NonPlayer();
            //entity.CreateFlags = EntityCreateFlag.SpawnAnimation;
            //entity.Initialise(npcModel, context.Invoker.Position, context.Invoker.Rotation);
            //// Cannot use "context.GetTargetOrInvoker()" because if you are targeting an NPC it returns the NPC
            //context.Invoker.Map.EnqueueAdd(entity, context.Invoker.Position);

            //// Add New Entity to the EntityCache
            //var cache = EntityCacheManager.Instance.GetEntityCache((ushort)context.Invoker.Map.Entry.Id);
            //cache.AddEntity(npcModel);
            //var activeNPCGrid = context.Invoker.Map.GetGrid(context.Invoker.Position);
            //var entityList = cache.GetEntities(activeNPCGrid.Coord.X, activeNPCGrid.Coord.Z).ToList();

            //// This function sets all the properties like health and shield etc, also prevents from spawning dead.
            //entity.CalculateProperties();

            //// Save Entity to Database (Entity Table)
            //DatabaseManager.Instance.WorldDatabase.SaveEntities(entityList);
            #endregion
        }

        // Disabling Delete. Entities spawned are not persistent for now.
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
            //var model = DatabaseManager.Instance.WorldDatabase
            //    .GetEntities((ushort)context.Invoker.Map.Entry.Id)
            //    .Where(x => x.Id == context.Target.EntityId)
            //    .FirstOrDefault();

            //// Remove Entity from Entity Cache (In-Game Object)
            //EntityCacheManager.Instance.GetEntityCache((ushort)context.Invoker.Map.Entry.Id).DeleteEntity(model);

            //// Remove Entity from Database (Persistence)
            //DatabaseManager.Instance.WorldDatabase.RemoveEntity(model);
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
