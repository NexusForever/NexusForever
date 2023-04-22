using NexusForever.Network.Message;
using NetworkGuildStandard = NexusForever.Network.World.Message.Model.Shared.GuildStandard;

namespace NexusForever.Game.Abstract.Guild
{
    public interface IGuildStandard : INetworkBuildable<NetworkGuildStandard>
    {
        IGuildStandardPart BackgroundIcon { get; }
        IGuildStandardPart ForegroundIcon { get; }
        IGuildStandardPart ScanLines { get; }

        /// <summary>
        /// Returns if <see cref="IGuildStandard"/> contains valid data.
        /// </summary>
        bool Validate();
    }
}