using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Who;
using Path = NexusForever.Game.Static.PlayerPath.Path;

namespace NexusForever.Network.Internal.Message.Who.Parameter
{
    public class WhoParameterCombo : IWhoParameter
    {
        public WhoParameterType Type => WhoParameterType.Combo;

        public string SearchString { get; set; }
        public Race? RaceId { get; set; }
        public Path? PathId { get; set; }
        public Class? ClassId { get; set; }
        public ushort? WorldZoneId { get; set; }
        public ushort? WorldZoneId2 { get; set; }
    }
}
