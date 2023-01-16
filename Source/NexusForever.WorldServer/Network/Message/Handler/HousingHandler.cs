using System;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Guild;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.TextFilter;
using NexusForever.WorldServer.Game.TextFilter.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class HousingHandler
    {
        [MessageHandler(GameMessageOpcode.ClientHousingResidencePrivacyLevel)]
        public static void HandleHousingSetPrivacyLevel(WorldSession session, ClientHousingSetPrivacyLevel housingSetPrivacyLevel)
        {
            if (session.Player.Map is not ResidenceMapInstance)
                throw new InvalidPacketValueException();

            if (session.Player.ResidenceManager.Residence == null)
                throw new InvalidPacketValueException();

            if (housingSetPrivacyLevel.PrivacyLevel == ResidencePrivacyLevel.Public)
                GlobalResidenceManager.Instance.RegisterResidenceVists(session.Player.ResidenceManager.Residence, session.Player);
            else
                GlobalResidenceManager.Instance.DeregisterResidenceVists(session.Player.ResidenceManager.Residence.Id);

            session.Player.ResidenceManager.SetResidencePrivacy(housingSetPrivacyLevel.PrivacyLevel);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingCrateAllDecor)]
        public static void HandleHousingCrateAllDecor(WorldSession session, ClientHousingCrateAllDecor housingCrateAllDecor)
        {
            if (session.Player.Map is not ResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            residenceMap.CrateAllDecor(housingCrateAllDecor.TargetResidence, session.Player);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingRemodel)]
        public static void HandleHousingRemodel(WorldSession session, ClientHousingRemodel housingRemodel)
        {
            if (session.Player.Map is not ResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            residenceMap.Remodel(housingRemodel.TargetResidence, session.Player, housingRemodel);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingDecorUpdate)]
        public static void HandleHousingDecorUpdate(WorldSession session, ClientHousingDecorUpdate housingDecorUpdate)
        {
            if (session.Player.Map is not ResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            residenceMap.DecorUpdate(session.Player, housingDecorUpdate);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingFlagsUpdate)]
        public static void HandleHousingFlagsUpdate(WorldSession session, ClientHousingFlagsUpdate flagsUpdate)
        {
            if (session.Player.Map is not ResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            residenceMap.UpdateResidenceFlags(flagsUpdate.TargetResidence, session.Player, flagsUpdate);
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
            if (session.Player.Map is not ResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            if (!TextFilterManager.Instance.IsTextValid(housingRenameProperty.Name)
                   || !TextFilterManager.Instance.IsTextValid(housingRenameProperty.Name, UserText.HousingResidenceName))
                throw new InvalidPacketValueException();

            residenceMap.RenameResidence(session.Player, housingRenameProperty.TargetResidence, housingRenameProperty.Name);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingRandomCommunityList)]
        public static void HandleHousingRandomCommunityList(WorldSession session, ClientHousingRandomCommunityList _)
        {
            GlobalResidenceManager.Instance.SendRandomVisitableCommunities(session);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingRandomResidenceList)]
        public static void HandleHousingRandomResidenceList(WorldSession session, ClientHousingRandomResidenceList _)
        {
            var serverHousingRandomResidenceList = new ServerHousingRandomResidenceList();
            foreach (PublicResidence residence in GlobalResidenceManager.Instance.GetRandomVisitableResidences())
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
            if (!(session.Player.Map is ResidenceMapInstance))
                throw new InvalidPacketValueException();

            if (!session.Player.CanTeleport())
                return;

            Residence residence;
            if (!string.IsNullOrEmpty(housingVisit.TargetResidenceName))
                residence = GlobalResidenceManager.Instance.GetResidenceByOwner(housingVisit.TargetResidenceName);
            else if (!string.IsNullOrEmpty(housingVisit.TargetCommunityName))
                residence = GlobalResidenceManager.Instance.GetCommunityByOwner(housingVisit.TargetCommunityName);
            else if (housingVisit.TargetResidence.ResidenceId != 0ul)
                residence = GlobalResidenceManager.Instance.GetResidence(housingVisit.TargetResidence.ResidenceId);
            else if (housingVisit.TargetCommunity.NeighbourhoodId != 0ul)
            {
                ulong residenceId = GlobalGuildManager.Instance.GetGuild<Community>(housingVisit.TargetCommunity.NeighbourhoodId)?.Residence?.Id ?? 0ul;
                residence = GlobalResidenceManager.Instance.GetResidence(residenceId);
            }
            else
                throw new NotImplementedException();

            if (residence == null)
            {
                //session.Player.SendGenericError();
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
            ResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance(residence.PropertyInfoId);
            session.Player.Rotation    = entrance.Rotation.ToEulerDegrees();
            session.Player.TeleportTo(new MapPosition
            {
                Info     = new MapInfo
                {
                    Entry      = entrance.Entry,
                    InstanceId = residence.Parent?.Id ?? residence.Id
                },
                Position = entrance.Position
            });
        }

        [MessageHandler(GameMessageOpcode.ClientHousingEditMode)]
        public static void HandleHousingEditMode(WorldSession session, ClientHousingEditMode housingEditMode)
        {
        }

        [MessageHandler(GameMessageOpcode.ClientHousingReturn)]
        public static void HandleHousingReturn(WorldSession session, ClientHousingReturn _)
        {
            // housing return button will only be visible on other residence maps
            Residence residence = session.Player.ResidenceManager.Residence;
            if (session.Player.Map is not ResidenceMapInstance
                || session.Player.Map == residence?.Map)
                throw new InvalidPacketValueException();

            // return player to correct residence instance
            ResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance(residence.PropertyInfoId);
            session.Player.Rotation    = entrance.Rotation.ToEulerDegrees();
            session.Player.TeleportTo(new MapPosition
            {
                Info     = new MapInfo
                {
                    Entry      = entrance.Entry,
                    InstanceId = residence.Parent?.Id ?? residence.Id
                },
                Position = entrance.Position
            });
        }

        [MessageHandler(GameMessageOpcode.ClientHousingPlacedResidencesList)]
        public static void HandleHousingPlacedResidencesList(WorldSession session, ClientHousingPlacedResidencesList _)
        {
            if (session.Player.Map is not ResidenceMapInstance)
                throw new InvalidPacketValueException();

            Community community = session.Player.GuildManager.GetGuild<Community>(GuildType.Community);
            if (community?.Residence == null)
                throw new InvalidPacketValueException();

            var housingPlacedResidencesList = new ServerHousingPlacedResidencesList();
            foreach (ResidenceChild residenceChild in community.Residence.GetChildren())
            {
                string owner = null;
                if (residenceChild.Residence.OwnerId.HasValue)
                    owner = CharacterManager.Instance.GetCharacterInfo(residenceChild.Residence.OwnerId.Value)?.Name;

                housingPlacedResidencesList.Residences.Add(new ServerHousingPlacedResidencesList.Residence
                {
                    RealmId       = WorldServer.RealmId,
                    ResidenceId   = residenceChild.Residence.Id,
                    PlayerName    = owner ?? "",
                    PropertyIndex = (uint)residenceChild.Residence.PropertyInfoId - 100
                });
            }

            session.EnqueueMessageEncrypted(housingPlacedResidencesList);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingCommunityRename)]
        public static void HandleHousingCommunityRename(WorldSession session, ClientHousingCommunityRename housingCommunityRename)
        {
            if (session.Player.Map is not ResidenceMapInstance)
                throw new InvalidPacketValueException();

            // ignore the value in the packet
            Community community = session.Player.GuildManager.GetGuild<Community>(GuildType.Community);
            if (community == null)
                throw new InvalidPacketValueException();

            HousingResult GetResult()
            {
                // client checks if the player has a rank of 0, this is the same
                if (community.LeaderId != session.Player.CharacterId)
                    return HousingResult.InvalidPermissions;

                if (!TextFilterManager.Instance.IsTextValid(housingCommunityRename.Name)
                    || !TextFilterManager.Instance.IsTextValid(housingCommunityRename.Name, UserText.HousingResidenceName))
                    return HousingResult.InvalidResidenceName;

                GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(2395);
                if (entry == null)
                    return HousingResult.Failed;

                bool canAfford;
                if (housingCommunityRename.AlternativeCurrency)
                    canAfford = session.Player.CurrencyManager.CanAfford(CurrencyType.Renown, entry.Dataint01);
                else
                    canAfford = session.Player.CurrencyManager.CanAfford(CurrencyType.Credits, entry.Dataint0);

                if (!canAfford)
                    return HousingResult.InsufficientFunds;

                return HousingResult.Success;
            }

            HousingResult result = GetResult();
            if (result == HousingResult.Success)
            {
                // fun fact: 2395 is the final game formula entry
                GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(2395);
                if (housingCommunityRename.AlternativeCurrency)
                    session.Player.CurrencyManager.CurrencySubtractAmount(CurrencyType.Renown, entry.Dataint01);
                else
                    session.Player.CurrencyManager.CurrencySubtractAmount(CurrencyType.Credits, entry.Dataint0);

                community.RenameGuild(housingCommunityRename.Name);
                community.Residence.Map?.RenameResidence(community.Residence, housingCommunityRename.Name);
            }

            session.EnqueueMessageEncrypted(new ServerHousingCommunityRename
            {
                Result      = HousingResult.Success,
                TargetGuild = new TargetGuild
                {
                    RealmId = WorldServer.RealmId,
                    GuildId = community.Id
                }
            });
        }

        [MessageHandler(GameMessageOpcode.ClientHousingCommunityPrivacyLevel)]
        public static void HandleHousingCommunityPrivacyLevel(WorldSession session, ClientHousingCommunityPrivacyLevel housingCommunityPrivacyLevel)
        {
            if (session.Player.Map is not ResidenceMapInstance)
                throw new InvalidPacketValueException();

            // ignore the value in the packet
            Community community = session.Player.GuildManager.GetGuild<Community>(GuildType.Community);
            if (community == null)
                throw new InvalidPacketValueException();

           if (!community.GetMember(session.Player.CharacterId).Rank.HasPermission(GuildRankPermission.ChangeCommunityRemodelOptions))
                throw new InvalidPacketValueException();

            if (housingCommunityPrivacyLevel.PrivacyLevel == CommunityPrivacyLevel.Public)
                GlobalResidenceManager.Instance.RegisterCommunityVisits(community.Residence, community, session.Player);
            else
                GlobalResidenceManager.Instance.DeregisterCommunityVists(community.Residence.Id);

            community.SetCommunityPrivate(housingCommunityPrivacyLevel.PrivacyLevel == CommunityPrivacyLevel.Private);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingCommunityDonate)]
        public static void HandleHousingCommunityDonate(WorldSession session, ClientHousingCommunityDonate housingCommunityDonate)
        {
            // can only donate to a community from a residence map
            if (session.Player.Map is not ResidenceMapInstance)
                throw new InvalidPacketValueException();

            Residence residence = session.Player.ResidenceManager.Residence;
            if (residence == null)
                throw new InvalidPacketValueException();

            Community community = session.Player.GuildManager.GetGuild<Community>(GuildType.Community);
            if (community?.Residence == null)
                throw new InvalidPacketValueException();

            foreach (DecorInfo decorInfo in housingCommunityDonate.Decor)
            {
                Decor decor = residence.GetDecor(decorInfo.DecorId);
                if (decor == null)
                    throw new InvalidPacketValueException();

                if (decor.Type != DecorType.Crate)
                    throw new InvalidPacketValueException();

                // copy decor to recipient residence
                if (community.Residence.Map != null)
                    community.Residence.Map.DecorCopy(community.Residence, decor);
                else
                    community.Residence.DecorCopy(decor);

                // remove decor from donor residence
                if (residence.Map != null)
                    residence.Map.DecorDelete(residence, decor);
                else
                {
                    if (decor.PendingCreate)
                        residence.DecorRemove(decor);
                    else
                        decor.EnqueueDelete();
                }
            }
        }

        [MessageHandler(GameMessageOpcode.ClientHousingCommunityPlacement)]
        public static void HandleHousingCommunityPlacement(WorldSession session, ClientHousingCommunityPlacement housingCommunityPlacement)
        {
            if (session.Player.Map is not ResidenceMapInstance)
                throw new InvalidPacketValueException();

            Community community = session.Player.GuildManager.GetGuild<Community>(GuildType.Community);
            if (community?.Residence == null)
                throw new InvalidPacketValueException();

            ResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance((PropertyInfoId)(housingCommunityPlacement.PropertyIndex + 100));
            if (entrance == null)
                throw new InvalidPacketValueException();

            Residence residence = session.Player.ResidenceManager.Residence;
            if (residence == null)
                throw new InvalidPacketValueException();

            if (residence.Parent != null)
            {
                if (community.Residence.GetChild(session.Player.CharacterId) == null)
                    throw new InvalidPacketValueException();

                // for residences on a community just remove the residence
                // any players on the map at the time can stay in the instance
                if (residence.Map != null)
                    residence.Map.RemoveChild(residence);
                else
                    residence.Parent.RemoveChild(residence);

                session.Player.Rotation = entrance.Rotation.ToEulerDegrees();
                session.Player.TeleportTo(entrance.Entry, entrance.Position, community.Residence.Id);
            }
            else
            {
                // move owner to new instance only if not on the same instance as the residence
                // otherwise they will be moved to the new instance during the unload
                if (residence.Map != session.Player.Map)
                {
                    session.Player.Rotation = entrance.Rotation.ToEulerDegrees();
                    session.Player.TeleportTo(entrance.Entry, entrance.Position, community.Residence.Id);
                }

                // for individual residences remove the entire instance
                // move any players on the map at the time to the community
                residence.Map?.Unload(new MapPosition
                {
                    Info     = new MapInfo
                    {
                        Entry      = entrance.Entry,
                        InstanceId = community.Residence.Id,
                    },
                    Position = entrance.Position
                });
            }

            // update residence with new plot location and add to community
            residence.PropertyInfoId = (PropertyInfoId)(housingCommunityPlacement.PropertyIndex + 100);

            if (community.Residence.Map != null)
                community.Residence.Map.AddChild(residence, true);
            else
                community.Residence.AddChild(residence, true);
        }

        // TODO: investigate why this doesn't get triggered on another housing plot
        // client has a global variable that is only set when receiving hosuing plots which isn't set when on another housing plot
        [MessageHandler(GameMessageOpcode.ClientHousingCommunityRemoval)]
        public static void HandleHousingCommunityRemoval(WorldSession session, ClientHousingCommunityRemoval housingCommunityRemoval)
        {
            if (session.Player.Map is not ResidenceMapInstance)
                throw new InvalidPacketValueException();

            Community community = session.Player.GuildManager.GetGuild<Community>(GuildType.Community);
            if (community?.Residence == null)
                throw new InvalidPacketValueException();

            community.RemoveChildResidence(session.Player);
        }
    }
}
