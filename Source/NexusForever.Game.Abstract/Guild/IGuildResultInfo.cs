using NexusForever.Game.Static.Guild;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Guild
{
    public interface IGuildResultInfo
    {
        GuildResult Result { get; set; }
        Identity GuildIdentity { get; set; }
        string ReferenceString { get; set; }
        uint ReferenceId { get; set; }
    }
}