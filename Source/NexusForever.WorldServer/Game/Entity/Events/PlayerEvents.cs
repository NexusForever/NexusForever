using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.Quest.Static;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Numerics;

namespace NexusForever.WorldServer.Game.Entity
{
    public partial class Player
    {
        private void OnLogin()
        {
            string motd = WorldServer.RealmMotd;
            if (motd?.Length > 0)
                GlobalChatManager.Instance.SendMessage(Session, motd, "MOTD", ChatChannelType.Realm);

            GuildManager.OnLogin();
            ChatManager.OnLogin();
        }

        private void OnLogout()
        {
            GuildManager.OnLogout();
            ChatManager.OnLogout();
        }

        public override void OnAddToMap(BaseMap map, uint guid, Vector3 vector)
        {
            IsLoading = true;

            Session.EnqueueMessageEncrypted(new ServerChangeWorld
            {
                WorldId = (ushort)map.Entry.Id,
                Position = new Position(vector)
            });

            // this must come before OnAddToMap
            // the client UI initialises the Holomark checkboxes during OnDocumentReady
            SendCharacterFlagsUpdated();

            base.OnAddToMap(map, guid, vector);
            map.OnAddToMap(this);

            // resummon vanity pet if it existed before teleport
            if (pendingTeleport?.VanityPetId != null)
            {
                var vanityPet = new VanityPet(this, pendingTeleport.VanityPetId.Value);
                map.EnqueueAdd(vanityPet, Position);
            }

            pendingTeleport = null;

            SendPacketsAfterAddToMap();
            if (PreviousMap == null)
                OnLogin();
        }

        public override void OnRemoveFromMap()
        {
            DestroyDependents();

            base.OnRemoveFromMap();

            if (pendingTeleport != null)
                MapManager.Instance.AddToMap(this, pendingTeleport.Info, pendingTeleport.Vector);
        }

        public override void OnRelocate(Vector3 vector)
        {
            base.OnRelocate(vector);
            saveMask |= PlayerSaveMask.Location;

            ZoneMapManager.OnRelocate(vector);
        }

        protected override void OnZoneUpdate()
        {
            if (Zone != null)
            {
                TextTable tt = GameTableManager.Instance.GetTextTable(Language.English);
                if (tt != null)
                {
                    GlobalChatManager.Instance.SendMessage(Session, $"New Zone: ({Zone.Id}){tt.GetEntry(Zone.LocalizedTextIdName)}");
                }

                uint tutorialId = AssetManager.Instance.GetTutorialIdForZone(Zone.Id);
                if (tutorialId > 0)
                {
                    Session.EnqueueMessageEncrypted(new ServerTutorial
                    {
                        TutorialId = tutorialId
                    });
                }

                QuestManager.ObjectiveUpdate(QuestObjectiveType.EnterZone, Zone.Id, 1);
            }

            ZoneMapManager.OnZoneUpdate();
        }

        /// <summary>
        /// Fires every time a regeneration tick occurs (every 0.5s)
        /// </summary>
        protected override void OnTickRegeneration()
        {
            base.OnTickRegeneration();

            float dashRemaining = GetStatFloat(Stat.Dash) ?? 0f;
            if (dashRemaining < GetPropertyValue(Property.ResourceMax7).Value)
            {
                float dashRegenAmount = GetPropertyValue(Property.ResourceMax7).Value * GetPropertyValue(Property.ResourceRegenMultiplier7).Value;
                SetStat(Stat.Dash, (float)Math.Min(dashRemaining + dashRegenAmount, (float)GetPropertyValue(Property.ResourceMax7).Value));
            }
        }
    }
}
