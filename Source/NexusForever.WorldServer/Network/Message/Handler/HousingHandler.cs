using System;
using System.Threading.Tasks;
using NexusForever.Shared.Game.Events;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class HousingHandler
    {
        [MessageHandler(GameMessageOpcode.ClientHousingResidencePrivacyLevel)]
        public static void HandleHousingSetPrivacyLevel(WorldSession session, ClientHousingSetPrivacyLevel housingSetPrivacyLevel)
        {
            if (!(session.Player.Map is ResidenceMap residenceMap))
                throw new InvalidPacketValueException();

            residenceMap.SetPrivacyLevel(session.Player, housingSetPrivacyLevel);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingCrateAllDecor)]
        public static void HandleHousingCrateAllDecor(WorldSession session, ClientHousingCrateAllDecor housingCrateAllDecor)
        {
            if (!(session.Player.Map is ResidenceMap residenceMap))
                throw new InvalidPacketValueException();

            residenceMap.CrateAllDecor(session.Player);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingRemodel)]
        public static void HandleHousingRemodel(WorldSession session, ClientHousingRemodel housingRemodel)
        {
            if (!(session.Player.Map is ResidenceMap residenceMap))
                throw new InvalidPacketValueException();

            residenceMap.Remodel(session.Player, housingRemodel);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingDecorUpdate)]
        public static void HandleHousingDecorUpdate(WorldSession session, ClientHousingDecorUpdate housingDecorUpdate)
        {
            if (!(session.Player.Map is ResidenceMap residenceMap))
                throw new InvalidPacketValueException();

            residenceMap.DecorUpdate(session.Player, housingDecorUpdate);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingPlugUpdate)]
        public static void HandleHousingPlugUpdate(WorldSession session, ClientHousingPlugUpdate housingPlugUpdate)
        {
            // TODO
        }

        [MessageHandler(GameMessageOpcode.ClientHousingVendorList)]
        public static void HandleHousingVendorList(WorldSession session, ClientHousingVendorList housingVendorList)
        {
            var serverHousingVendorList = new ServerHousingVendorList
            {
                ListType = 0
            };
            
            // TODO: this isn't entirely correct
            foreach (HousingPlugItemEntry entry in GameTableManager.Instance.HousingPlugItem.Entries)
            {
                serverHousingVendorList.PlugItems.Add(new ServerHousingVendorList.PlugItem
                {
                    PlugItemId = entry.Id
                });
            }
            
            session.EnqueueMessageEncrypted(serverHousingVendorList);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingRenameProperty)]
        public static void HandleHousingRenameProperty(WorldSession session, ClientHousingRenameProperty housingRenameProperty)
        {
            if (!(session.Player.Map is ResidenceMap residenceMap))
                throw new InvalidPacketValueException();

            // TODO: validate name
            residenceMap.Rename(session.Player, housingRenameProperty);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingRandomCommunityList)]
        public static void HandleHousingRandomCommunityList(WorldSession session, ClientHousingRandomCommunityList housingRandomCommunityList)
        {
            session.EnqueueMessageEncrypted(new ServerHousingRandomCommunityList
            {
                Communities =
                {
                    new ServerHousingRandomCommunityList.Community
                    {
                        RealmId        = WorldServer.RealmId,
                        NeighborhoodId = 123,
                        Name           = "Blame Maxtor for working on WoW instead",
                        Owner          = "Not Yet Implemented!"
                    }
                }
            });
        }

        [MessageHandler(GameMessageOpcode.ClientHousingRandomResidenceList)]
        public static void HandleHousingRandomResidenceList(WorldSession session, ClientHousingRandomResidenceList housingRandomResidenceList)
        {
            var serverHousingRandomResidenceList = new ServerHousingRandomResidenceList();
            foreach (PublicResidence residence in ResidenceManager.Instance.GetRandomVisitableResidences())
            {
                serverHousingRandomResidenceList.Residences.Add(new ServerHousingRandomResidenceList.Residence
                {
                    RealmId     = WorldServer.RealmId,
                    ResidenceId = residence.ResidenceId,
                    Owner       = residence.Owner,
                    Name        = residence.Name
                });
            }

            session.EnqueueMessageEncrypted(serverHousingRandomResidenceList);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingVisit)]
        public static void HandleHousingVisit(WorldSession session, ClientHousingVisit housingVisit)
        {
            if (!(session.Player.Map is ResidenceMap))
                throw new InvalidPacketValueException();

            Task<Residence> residenceTask;
            if (housingVisit.TargetResidenceName != "")
                residenceTask = ResidenceManager.Instance.GetResidence(housingVisit.TargetResidenceName);
            else if (housingVisit.TargetResidence.ResidenceId != 0ul)
                residenceTask = ResidenceManager.Instance.GetResidence(housingVisit.TargetResidence.ResidenceId);
            else
                throw new NotImplementedException();

            session.EnqueueEvent(new TaskGenericEvent<Residence>(residenceTask,
                residence =>
            {
                if (residence == null)
                {
                    // TODO: show error
                    return;
                }

                switch (residence.PrivacyLevel)
                {
                    case ResidencePrivacyLevel.Private:
                    {
                        // TODO: show error
                        return;
                    }
                    // TODO: check if player is either a neighbour or roommate
                    case ResidencePrivacyLevel.NeighborsOnly:
                        break;
                    case ResidencePrivacyLevel.RoommatesOnly:
                        break;
                }

                // teleport player to correct residence instance
                ResidenceEntrance entrance = ResidenceManager.Instance.GetResidenceEntrance(residence);
                session.Player.TeleportTo(entrance.Entry, entrance.Position, 0u, residence.Id);
            }));
        }

        [MessageHandler(GameMessageOpcode.ClientHousingEditMode)]
        public static void HandleHousingEditMode(WorldSession session, ClientHousingEditMode housingEditMode)
        {
        }
    }
}
