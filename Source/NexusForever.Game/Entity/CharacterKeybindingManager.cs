using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Setting;
using NexusForever.Game.Setting;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity
{
    public class CharacterKeybindingManager : ICharacterKeybindingManager
    {
        private readonly IPlayer player;
        private readonly IKeybindingSet bindingSet;

        public CharacterKeybindingManager(IPlayer player, CharacterModel model)
        {
            this.player = player;
            bindingSet  = new KeybindingSet(model);
        }

        public void Save(CharacterContext context)
        {
            bindingSet.Save(context);
        }

        public void Update(BiInputKeySet inputKeySet)
        {
            bindingSet.Update(inputKeySet);
        }

        public BiInputKeySet Build()
        {
            return new BiInputKeySet
            {
                Bindings    = bindingSet.Select(b => b.Build()).ToList(),
                CharacterId = player.CharacterId
            };
        }
    }
}
