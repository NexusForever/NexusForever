using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Setting
{
    public interface IKeybinding : IDatabaseCharacter, IDatabaseState, IDatabaseAuth, INetworkBuildable<Binding>
    {
        uint Code00 { get; set; }
        uint Code01 { get; set; }
        uint Code02 { get; set; }
        uint DeviceEnum00 { get; set; }
        uint DeviceEnum01 { get; set; }
        uint DeviceEnum02 { get; set; }
        uint EventTypeEnum00 { get; set; }
        uint EventTypeEnum01 { get; set; }
        uint EventTypeEnum02 { get; set; }
        ushort InputActionId { get; set; }
        uint MetaKeys00 { get; set; }
        uint MetaKeys01 { get; set; }
        uint MetaKeys02 { get; set; }
        ulong Owner { get; }

        void Update(Binding networkBinding);
    }
}