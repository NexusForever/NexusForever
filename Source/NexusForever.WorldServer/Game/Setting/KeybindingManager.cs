using System.Collections.Generic;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Setting.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NetworkBinding = NexusForever.WorldServer.Network.Message.Model.Shared.Binding;

namespace NexusForever.WorldServer.Game.Setting
{
    public class KeybindingManager : ISaveAuth, ISaveCharacter
    {
        private readonly Player player;
        private readonly KeybindingSet accountKeybindings;
        private readonly KeybindingSet characterKeybindings;

        public KeybindingManager(Player owner, AccountModel accountModel, CharacterModel characterModel)
        {
            player               = owner;
            accountKeybindings   = new KeybindingSet(accountModel);
            characterKeybindings = new KeybindingSet(characterModel);
        }

        public void Save(AuthContext context)
        {
            accountKeybindings.Save(context);
        }

        public void Save(CharacterContext context)
        {
            characterKeybindings.Save(context);
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
            var networkBindings = new List<NetworkBinding>();
            foreach (Keybinding binding in player.InputKeySet == InputSets.Character ? characterKeybindings : accountKeybindings)
                networkBindings.Add(BuildNetworkBinding(binding));
                
            player.Session.EnqueueMessageEncrypted(new BiInputKeySet
            {
                Bindings    = networkBindings,
                CharacterId = player.InputKeySet == InputSets.Character ? player.CharacterId : 0ul
            });
        }

        public void SendInputKeySet(ulong characterId)
        {
            var networkBindings = new List<NetworkBinding>();
            foreach (Keybinding binding in characterId != 0ul ? characterKeybindings : accountKeybindings)
                networkBindings.Add(BuildNetworkBinding(binding));
                
            player.Session.EnqueueMessageEncrypted(new BiInputKeySet
            {
                Bindings    = networkBindings,
                CharacterId = player.InputKeySet == InputSets.Character ? player.CharacterId : 0ul
            });
        }

        public void SendInitialPackets()
        {
            if (characterKeybindings.Count + accountKeybindings.Count == 0u)
                return;

            SendInputKeySet();
        }

        private NetworkBinding BuildNetworkBinding(Keybinding binding)
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
