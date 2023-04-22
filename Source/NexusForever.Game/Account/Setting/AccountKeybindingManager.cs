using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.Account.Setting;
using NexusForever.Game.Abstract.Setting;
using NexusForever.Game.Setting;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Account.Setting
{
    public class AccountKeybindingManager : IAccountKeybindingManager
    {
        private readonly IKeybindingSet bindingSet;

        public AccountKeybindingManager(AccountModel model)
        {
            bindingSet = new KeybindingSet(model);
        }

        public void Save(AuthContext context)
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
                CharacterId = 0ul
            };
        }
    }
}
