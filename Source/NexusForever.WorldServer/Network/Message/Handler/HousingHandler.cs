using System;
using NexusForever.Game;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Character;
using NexusForever.Game.Guild;
using NexusForever.Game.Housing;
using NexusForever.Game.Map;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Housing;
using NexusForever.Game.Static.TextFilter;
using NexusForever.Game.Text.Filter;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class HousingHandler
    {
        [MessageHandler(GameMessageOpcode.ClientHousingResidencePrivacyLevel)]
        public static void HandleHousingSetPrivacyLevel(IWorldSession session, ClientHousingSetPrivacyLevel housingSetPrivacyLevel)
        {
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            if (session.Player.ResidenceManager.Residence == null)
                throw new InvalidPacketValueException();

            if (housingSetPrivacyLevel.PrivacyLevel == ResidencePrivacyLevel.Public)
                GlobalResidenceManager.Instance.RegisterResidenceVists(session.Player.ResidenceManager.Residence, session.Player.Name);
            else
                GlobalResidenceManager.Instance.DeregisterResidenceVists(session.Player.ResidenceManager.Residence.Id);

            session.Player.ResidenceManager.SetResidencePrivacy(housingSetPrivacyLevel.PrivacyLevel);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingCrateAllDecor)]
        public static void HandleHousingCrateAllDecor(IWorldSession session, ClientHousingCrateAllDecor housingCrateAllDecor)
        {
            if (session.Player.Map is not IResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            residenceMap.CrateAllDecor(housingCrateAllDecor.TargetResidence, session.Player);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingRemodel)]
        public static void HandleHousingRemodel(IWorldSession session, ClientHousingRemodel housingRemodel)
        {
            if (session.Player.Map is not IResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            residenceMap.Remodel(housingRemodel.TargetResidence, session.Player, housingRemodel);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingDecorUpdate)]
        public static void HandleHousingDecorUpdate(IWorldSession session, ClientHousingDecorUpdate housingDecorUpdate)
        {
            if (session.Player.Map is not IResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            residenceMap.DecorUpdate(session.Player, housingDecorUpdate);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingFlagsUpdate)]
        public static void HandleHousingFlagsUpdate(IWorldSession session, ClientHousingFlagsUpdate flagsUpdate)
        {
            if (session.Player.Map is not IResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            residenceMap.UpdateResidenceFlags(flagsUpdate.TargetResidence, session.Player, flagsUpdate);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingPlugUpdate)]
        public static void HandleHousingPlugUpdate(IWorldSession session, ClientHousingPlugUpdate housingPlugUpdate)
        {
            // TODO
        }

        [MessageHandler(GameMessageOpcode.ClientHousingVendorList)]
        public static void HandleHousingVendorList(IWorldSession session, ClientHousingVendorList housingVendorList)
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
        public static void HandleHousingRenameProperty(IWorldSession session, ClientHousingRenameProperty housingRenameProperty)
        {
            if (session.Player.Map is not IResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            if (!TextFilterManager.Instance.IsTextValid(housingRenameProperty.Name)
                   || !TextFilterManager.Instance.IsTextValid(housingRenameProperty.Name, UserText.HousingResidenceName))
                throw new InvalidPacketValueException();

            residenceMap.RenameResidence(session.Player, housingRenameProperty.TargetResidence, housingRenameProperty.Name);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingRandomCommunityList)]
        public static void HandleHousingRandomCommunityList(IWorldSession session, ClientHousingRandomCommunityList _)
        {
            var serverHousingRandomCommunityList = new ServerHousingRandomCommunityList();
            foreach (IPublicCommunity community in GlobalResidenceManager.Instance.GetRandomVisitableCommunities())
            {
                serverHousingRandomCommunityList.Communities.Add(new ServerHousingRandomCommunityList.Community
                {
                    RealmId        = RealmContext.Instance.RealmId,
                    NeighborhoodId = community.NeighbourhoodId,
                    Owner          = community.Owner,
                    Name           = community.Name
                });
            }

            session.EnqueueMessageEncrypted(serverHousingRandomCommunityList);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingRandomResidenceList)]
        public static void HandleHousingRandomResidenceList(IWorldSession session, ClientHousingRandomResidenceList _)
        {
            var serverHousingRandomResidenceList = new ServerHousingRandomResidenceList();
            foreach (IPublicResidence residence in GlobalResidenceManager.Instance.GetRandomVisitableResidences())
            {
                serverHousingRandomResidenceList.Residences.Add(new ServerHousingRandomResidenceList.Residence
                {
                    RealmId     = RealmContext.Instance.RealmId,
                    ResidenceId = residence.ResidenceId,
                    Owner       = residence.Owner,
                    Name        = residence.Name
                });
            }

            session.EnqueueMessageEncrypted(serverHousingRandomResidenceList);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingVisit)]
        public static void HandleHousingVisit(IWorldSession session, ClientHousingVisit housingVisit)
        {
            if (!(session.Player.Map is IResidenceMapInstance))
                throw new InvalidPacketValueException();

            if (!session.Player.CanTeleport())
                return;

            IResidence residence;
            if (!string.IsNullOrEmpty(housingVisit.TargetResidenceName))
                residence = GlobalResidenceManager.Instance.GetResidenceByOwner(housingVisit.TargetResidenceName);
            else if (!string.IsNullOrEmpty(housingVisit.TargetCommunityName))
                residence = GlobalResidenceManager.Instance.GetCommunityByOwner(housingVisit.TargetCommunityName);
            else if (housingVisit.TargetResidence.ResidenceId != 0ul)
                residence = GlobalResidenceManager.Instance.GetResidence(housingVisit.TargetResidence.ResidenceId);
            else if (housingVisit.TargetCommunity.NeighbourhoodId != 0ul)
            {
                ulong residenceId = GlobalGuildManager.Instance.GetGuild<ICommunity>(housingVisit.TargetCommunity.NeighbourhoodId)?.Residence?.Id ?? 0ul;
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
            IResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance(residence.PropertyInfoId);
            session.Player.Rotation     = entrance.Rotation.ToEulerDegrees();
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
        public static void HandleHousingEditMode(IWorldSession session, ClientHousingEditMode housingEditMode)
        {
        }

        [MessageHandler(GameMessageOpcode.ClientHousingReturn)]
        public static void HandleHousingReturn(IWorldSession session, ClientHousingReturn _)
        {
            // housing return button will only be visible on other residence maps
            IResidence residence = session.Player.ResidenceManager.Residence;
            if (session.Player.Map is not IResidenceMapInstance
                || session.Player.Map == residence?.Map)
                throw new InvalidPacketValueException();

            // return player to correct residence instance
            IResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance(residence.PropertyInfoId);
            session.Player.Rotation     = entrance.Rotation.ToEulerDegrees();
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
        public static void HandleHousingPlacedResidencesList(IWorldSession session, ClientHousingPlacedResidencesList _)
        {
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            ICommunity community = session.Player.GuildManager.GetGuild<ICommunity>(GuildType.Community);
            if (community?.Residence == null)
                throw new InvalidPacketValueException();

            var housingPlacedResidencesList = new ServerHousingPlacedResidencesList();
            foreach (IResidenceChild residenceChild in community.Residence.GetChildren())
            {
                string owner = null;
                if (residenceChild.Residence.OwnerId.HasValue)
                    owner = CharacterManager.Instance.GetCharacter(residenceChild.Residence.OwnerId.Value)?.Name;

                housingPlacedResidencesList.Residences.Add(new ServerHousingPlacedResidencesList.Residence
                {
                    RealmId       = RealmContext.Instance.RealmId,
                    ResidenceId   = residenceChild.Residence.Id,
                    PlayerName    = owner ?? "",
                    PropertyIndex = (uint)residenceChild.Residence.PropertyInfoId - 100
                });
            }

            session.EnqueueMessageEncrypted(housingPlacedResidencesList);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingCommunityRename)]
        public static void HandleHousingCommunityRename(IWorldSession session, ClientHousingCommunityRename housingCommunityRename)
        {
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            // ignore the value in the packet
            ICommunity community = session.Player.GuildManager.GetGuild<ICommunity>(GuildType.Community);
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
                    RealmId = RealmContext.Instance.RealmId,
                    GuildId = community.Id
                }
            });
        }

        [MessageHandler(GameMessageOpcode.ClientHousingCommunityPrivacyLevel)]
        public static void HandleHousingCommunityPrivacyLevel(IWorldSession session, ClientHousingCommunityPrivacyLevel housingCommunityPrivacyLevel)
        {
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            // ignore the value in the packet
            ICommunity community = session.Player.GuildManager.GetGuild<ICommunity>(GuildType.Community);
            if (community == null)
                throw new InvalidPacketValueException();

            if (!community.GetMember(session.Player.CharacterId).Rank.HasPermission(GuildRankPermission.ChangeCommunityRemodelOptions))
                throw new InvalidPacketValueException();

            if (housingCommunityPrivacyLevel.PrivacyLevel == CommunityPrivacyLevel.Public)
                GlobalResidenceManager.Instance.RegisterCommunityVisits(community.Residence, community, session.Player.Name);
            else
                GlobalResidenceManager.Instance.DeregisterCommunityVists(community.Residence.Id);

            community.SetCommunityPrivate(housingCommunityPrivacyLevel.PrivacyLevel == CommunityPrivacyLevel.Private);
        }

        [MessageHandler(GameMessageOpcode.ClientHousingCommunityDonate)]
        public static void HandleHousingCommunityDonate(IWorldSession session, ClientHousingCommunityDonate housingCommunityDonate)
        {
            // can only donate to a community from a residence map
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            IResidence residence = session.Player.ResidenceManager.Residence;
            if (residence == null)
                throw new InvalidPacketValueException();

            ICommunity community = session.Player.GuildManager.GetGuild<ICommunity>(GuildType.Community);
            if (community?.Residence == null)
                throw new InvalidPacketValueException();

            foreach (DecorInfo decorInfo in housingCommunityDonate.Decor)
            {
                IDecor decor = residence.GetDecor(decorInfo.DecorId);
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
                        decor.EnqueueDelete(true);
                }
            }
        }

        [MessageHandler(GameMessageOpcode.ClientHousingCommunityPlacement)]
        public static void HandleHousingCommunityPlacement(IWorldSession session, ClientHousingCommunityPlacement housingCommunityPlacement)
        {
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            ICommunity community = session.Player.GuildManager.GetGuild<ICommunity>(GuildType.Community);
            if (community?.Residence == null)
                throw new InvalidPacketValueException();

            IResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance((PropertyInfoId)(housingCommunityPlacement.PropertyIndex + 100));
            if (entrance == null)
                throw new InvalidPacketValueException();

            IResidence residence = session.Player.ResidenceManager.Residence;
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
        public static void HandleHousingCommunityRemoval(IWorldSession session, ClientHousingCommunityRemoval housingCommunityRemoval)
        {
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            ICommunity community = session.Player.GuildManager.GetGuild<ICommunity>(GuildType.Community);
            if (community?.Residence == null)
                throw new InvalidPacketValueException();

            IResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance(PropertyInfoId.Residence);
            if (entrance == null)
                throw new InvalidOperationException();

            IResidenceChild child = community.Residence.GetChild(session.Player.CharacterId);
            if (child == null)
                throw new InvalidOperationException();

            if (child.Residence.Map != null)
                child.Residence.Map.RemoveChild(child.Residence);
            else
                child.Residence.Parent.RemoveChild(child.Residence);

            child.Residence.PropertyInfoId = PropertyInfoId.Residence;

            // shouldn't need to check for existing instance
            // individual residence instances are unloaded when transfered to a community
            // if for some reason the instance is still unloading the residence will be initalised again after
            session.Player.Rotation = entrance.Rotation.ToEulerDegrees();
            session.Player.TeleportTo(entrance.Entry, entrance.Position, child.Residence.Id);
        }
    }
}
