using System.Text.Json.Serialization;
using NexusForever.Game.Static.Who;

namespace NexusForever.Network.Internal.Message.Who.Parameter
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(WhoParameterClass), nameof(WhoParameterType.Class))]
    [JsonDerivedType(typeof(WhoParameterCombo), nameof(WhoParameterType.Combo))]
    [JsonDerivedType(typeof(WhoParameterFaction), nameof(WhoParameterType.Faction))]
    [JsonDerivedType(typeof(WhoParameterGuild), nameof(WhoParameterType.Guild))]
    [JsonDerivedType(typeof(WhoParameterLevel), nameof(WhoParameterType.Level))]
    [JsonDerivedType(typeof(WhoParameterPath), nameof(WhoParameterType.Path))]
    [JsonDerivedType(typeof(WhoParameterPlayer), nameof(WhoParameterType.Player))]
    [JsonDerivedType(typeof(WhoParameterRace), nameof(WhoParameterType.Race))]
    [JsonDerivedType(typeof(WhoParameterZone), nameof(WhoParameterType.Zone))]
    public interface IWhoParameter
    {
        public WhoParameterType Type { get; }
    }
}
