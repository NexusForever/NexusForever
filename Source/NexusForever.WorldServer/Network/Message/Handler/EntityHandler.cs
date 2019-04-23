using System.Linq;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Command;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class EntityHandler
    {
        [MessageHandler(GameMessageOpcode.ClientEntityCommand)]
        public static void HandleClientEntityCommand(WorldSession session, ClientEntityCommand entityCommand)
        {
            WorldEntity mover = session.Player;
            if (session.Player.ControlGuid != session.Player.Guid)
                mover = session.Player.GetVisible<WorldEntity>(session.Player.ControlGuid);

            foreach ((EntityCommand id, IEntityCommandModel command) in entityCommand.Commands)
            {
                switch (command)
                {
                    case SetPositionCommand setPosition:
                    {
                        // this is causing issues after moving to soon after mounting:
                        // session.Player.CancelSpellsOnMove();

                        mover.Map.EnqueueRelocate(mover, setPosition.Position.Vector);
                        break;
                    }
                    case SetRotationCommand setRotation:
                        mover.Rotation = setRotation.Position.Vector;
                        break;
                }
            }

            mover.EnqueueToVisible(new ServerEntityCommand
            {
                Guid     = mover.Guid,
                Time     = entityCommand.Time,
                ServerControlled = true,
                Commands = entityCommand.Commands
            });
        }

        [MessageHandler(GameMessageOpcode.ClientActivateUnit)]
        public static void HandleClientEntityCommand(WorldSession session, ClientActivateUnit unit)
        {
            if (unit.UnitId < 1)
                throw new InvalidPacketValueException();

            var entity = session.Player.Map.GetEntity<GridEntity>(unit.UnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            // TODO: sanity check for range etc.

            if (!(entity is Simple simple)) 
                return;

            Creature2Entry creatureEntry = GameTableManager.Creature2.GetEntry(simple.CreatureId);
            if (creatureEntry == null)
                throw new InvalidPacketValueException();

            if (creatureEntry.DatacubeId <= 0) 
                return;

            var datacube = new Datacube
            {
                DatacubeId = (ushort)creatureEntry.DatacubeId ,
                Progress = int.MaxValue,
                saveMask = DatacubeSaveMask.Create
            };

            session.Player.DatacubeManager.AddDatacube(datacube);
        }

        [MessageHandler(GameMessageOpcode.ClientActivateUnitCast)]
        public static void HandleClientEntityCommand(WorldSession session, ClientActivateUnitCast unit)
        {
            if (unit.ActivateUnitId < 1)
                throw new InvalidPacketValueException();

            var entity = session.Player.Map.GetEntity<GridEntity>(unit.ActivateUnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            // TODO: sanity check for range etc.

            if (!(entity is Simple simple)) 
                return;

            Creature2Entry creatureEntry = GameTableManager.Creature2.GetEntry(simple.CreatureId);
            if (creatureEntry == null)
                throw new InvalidPacketValueException();

            // this is for tales keys / journals - there are other "simple" units to interact with though
            if (creatureEntry.DatacubeId <= 0 && creatureEntry.DatacubeVolumeId <= 0) 
                return;

            uint progess = (uint)(1 << simple.QuestChecklistIdx);

            Datacube datacube;
            if (creatureEntry.DatacubeId > 0)
                datacube = session.Player.DatacubeManager.DatacubeData.FirstOrDefault(c => c.DatacubeId == creatureEntry.DatacubeId);
            else if (creatureEntry.DatacubeVolumeId > 0)
                datacube = session.Player.DatacubeManager.DatacubeVolumeData.FirstOrDefault(c => c.DatacubeId == creatureEntry.DatacubeVolumeId);
            else
                throw new InvalidPacketValueException();

            if (datacube != null)
                datacube.saveMask |= DatacubeSaveMask.Modify;
            else
            {
                datacube = new Datacube
                {
                    DatacubeId = (ushort)(creatureEntry.DatacubeId > 0 ? creatureEntry.DatacubeId : creatureEntry.DatacubeVolumeId),
                    Progress = 0,
                    saveMask = DatacubeSaveMask.Create
                };
            }

            datacube.Progress |= (uint)(1 << simple.QuestChecklistIdx);

            if (creatureEntry.DatacubeId > 0)
            {
                if ((datacube.saveMask & DatacubeSaveMask.Create) != 0)
                    session.Player.DatacubeManager.AddDatacube(datacube);
                else
                    session.Player.DatacubeManager.SendDatacube(datacube);

            }
            else if (creatureEntry.DatacubeVolumeId > 0)
            {
                if ((datacube.saveMask & DatacubeSaveMask.Create) != 0)
                    session.Player.DatacubeManager.AddDatacubeVolume(datacube);
                else
                    session.Player.DatacubeManager.SendDatacubeVolume(datacube);
            }

            //TODO: cast "116,Generic Quest Spell - Activating - Activate - Tier 1" by 0x07FD
        }
    }
}
