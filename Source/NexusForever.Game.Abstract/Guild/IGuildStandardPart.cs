using NexusForever.Game.Static.Guild;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NetworkGuildStandard = NexusForever.Network.World.Message.Model.Shared.GuildStandard;

namespace NexusForever.Game.Abstract.Guild
{
    public interface IGuildStandardPart : INetworkBuildable<NetworkGuildStandard.GuildStandardPart>
    {
        GuildStandardPartType Type { get; }
        GuildStandardPartEntry GuildStandardPartEntry { get; }
        ushort DyeColorRampId1 { get; }
        ushort DyeColorRampId2 { get; }
        ushort DyeColorRampId3 { get; }

        /// <summary>
        /// Returns if <see cref="IGuildStandardPart"/> contains valid data.
        /// </summary>
        bool Validate();
    }
}