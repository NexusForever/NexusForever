using NexusForever.Database.Auth;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Account.Setting
{
    public interface IAccountKeybindingManager : IDatabaseAuth, INetworkBuildable<BiInputKeySet>
    {
        void Update(BiInputKeySet inputKeySet);
    }
}
