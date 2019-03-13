using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.Shared.Database.Auth.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Setting.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NetworkBinding = NexusForever.WorldServer.Network.Message.Model.Shared.Binding;

namespace NexusForever.WorldServer.Game.Setting
{
    public class KeybindingManager : ISaveAuth, ISaveCharacter, IUpdate
    {
        private readonly Player player;
        private Keybinding accountKeybindings;
        private Keybinding characterKeybindings;

        public KeybindingManager(Player owner, Account accountModel, Character characterModel)
        {
            player = owner;
            accountKeybindings   = new Keybinding(accountModel);
            characterKeybindings = new Keybinding(characterModel);
        }

        public void Save(AuthContext context)
        {
            accountKeybindings.Save(context);
        }

        public void Save(CharacterContext context)
        {
            characterKeybindings.Save(context);
        }

        public void Update(double lastTick)
        {
        }

        public void SaveKeybinding(BiInputKeySet biInputKeySet)
        {
            if (biInputKeySet.CharacterId == 0)
                accountKeybindings.Update(biInputKeySet);
            else
                characterKeybindings.Update(biInputKeySet);
        }

        public void SendInputKeySet()
        {
            List <NetworkBinding> networkBindings = new List<NetworkBinding>();
            foreach (Binding binding in player.InputKeySet == InputSets.Character ? characterKeybindings.bindings.Values : accountKeybindings.bindings.Values)
                networkBindings.Add(BuildNetworkBinding(binding));
                
            player.Session.EnqueueMessageEncrypted(new BiInputKeySet
            {
                Bindings = networkBindings,
                CharacterId = player.InputKeySet == InputSets.Character ? player.CharacterId : 0,
            });
        }

        public void SendInputKeySet(ulong characterId)
        {
            List <NetworkBinding> networkBindings = new List<NetworkBinding>();
            foreach (Binding binding in player.InputKeySet == InputSets.Character ? characterKeybindings.bindings.Values : accountKeybindings.bindings.Values)
            {
                networkBindings.Add(BuildNetworkBinding(binding));
            }
                
            player.Session.EnqueueMessageEncrypted(new BiInputKeySet
            {
                Bindings = networkBindings,
                CharacterId = player.InputKeySet == InputSets.Character ? player.CharacterId : 0,
            });
        }

        public void SendInitialPackets()
        {
            if (characterKeybindings.bindings.Count + accountKeybindings.bindings.Count == 0)
                return;

            SendInputKeySet();
        }

        private NetworkBinding BuildNetworkBinding(Binding binding)
        {
            var networkBinding = new NetworkBinding
            {
                InputActionId   = binding.InputActionId,
                DeviceEnum00    = binding.DeviceEnum00,
                DeviceEnum01    = binding.DeviceEnum01,
                DeviceEnum02    = binding.DeviceEnum02,
                Code00          = binding.Code00,
                Code01          = binding.Code01,
                Code02          = binding.Code02,
                MetaKeys00      = binding.MetaKeys00,
                MetaKeys01      = binding.MetaKeys01,
                MetaKeys02      = binding.MetaKeys02,
                EventTypeEnum00 = binding.EventTypeEnum00,
                EventTypeEnum01 = binding.EventTypeEnum01,
                EventTypeEnum02 = binding.EventTypeEnum02
            };

            return networkBinding;
        }
    }
}
