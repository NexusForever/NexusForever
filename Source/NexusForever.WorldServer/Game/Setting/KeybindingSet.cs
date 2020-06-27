using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Setting.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NetworkBinding = NexusForever.WorldServer.Network.Message.Model.Shared.Binding;

namespace NexusForever.WorldServer.Game.Setting
{
    public class KeybindingSet : ISaveCharacter, ISaveAuth, IEnumerable<Keybinding>
    {
        public ulong Owner { get; }
        public InputSets InputSet { get; }
        public uint Count => (uint)bindings.Count;

        private bool isDirty;

        private readonly Dictionary<ushort, Keybinding> bindings = new Dictionary<ushort, Keybinding>();

        /// <summary>
        /// Create a new <see cref="KeybindingSet"/> from an existing database model.
        /// </summary>
        public KeybindingSet(CharacterModel model)
        {
            Owner    = model.Id;
            InputSet = InputSets.Character;

            foreach (CharacterKeybindingModel binding in model.Keybinding)
                bindings.Add(binding.InputActionId, new Keybinding(Owner, binding));
        }

        /// <summary>
        /// Create a new <see cref="KeybindingSet"/> from an existing database model.
        /// </summary>
        public KeybindingSet(AccountModel model)
        {
            Owner    = model.Id;
            InputSet = InputSets.Account;

            foreach (AccountKeybindingModel binding in model.AccountKeybinding)
                bindings.Add(binding.InputActionId, new Keybinding(Owner, binding));
        }

        public void Save(CharacterContext context)
        {
            if (!isDirty)
                return;

            foreach (Keybinding binding in bindings.Values.ToList())
            {
                if (binding.PendingDelete)
                    bindings.Remove(binding.InputActionId);

                binding.Save(context);
            }

            isDirty = false;
        }

        public void Save(AuthContext context)
        {
            if (!isDirty)
                return;

            foreach (Keybinding binding in bindings.Values.ToList())
            {
                if (binding.PendingDelete)
                    bindings.Remove(binding.InputActionId);

                binding.Save(context);
            }

            isDirty = false;
        }

        /// <summary>
        /// Update <see cref="KeybindingSet"/> with information from supplied <see cref="BiInputKeySet"/> from client.
        /// </summary>
        public void Update(BiInputKeySet biInputKeySet)
        {
            if (bindings.Count + biInputKeySet.Bindings.Count == 0)
                return;

            isDirty = true;

            foreach (ushort inputActionId in bindings.Keys
                .Except(biInputKeySet.Bindings.Select(b => b.InputActionId)))
            {
                Keybinding binding = bindings[inputActionId];
                if (binding.PendingCreate)
                    bindings.Remove(inputActionId);
                else
                    binding.EnqueueDelete(true);
            }

            foreach (NetworkBinding networkBinding in biInputKeySet.Bindings)
            {
                if (!bindings.TryGetValue(networkBinding.InputActionId, out Keybinding binding))
                    bindings.Add(networkBinding.InputActionId, new Keybinding(Owner, networkBinding));
                else
                {
                    if (binding.PendingDelete)
                        binding.EnqueueDelete(false);

                    binding.Update(networkBinding);
                }   
            }
        }

        public IEnumerator<Keybinding> GetEnumerator()
        {
            return bindings.Values.Where(b => !b.PendingDelete).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
