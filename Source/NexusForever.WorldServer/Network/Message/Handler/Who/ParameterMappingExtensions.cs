using WhoParameterLevelNetwork = NexusForever.Network.World.Message.Model.Who.Parameter.WhoParameterLevel;
using WhoParameterLevelInternal = NexusForever.Network.Internal.Message.Who.Parameter.WhoParameterLevel;
using WhoParameterRaceNetwork = NexusForever.Network.World.Message.Model.Who.Parameter.WhoParameterRace;
using WhoParameterRaceInternal = NexusForever.Network.Internal.Message.Who.Parameter.WhoParameterRace;
using WhoParameterPathNetwork = NexusForever.Network.World.Message.Model.Who.Parameter.WhoParameterPath;
using WhoParameterPathInternal = NexusForever.Network.Internal.Message.Who.Parameter.WhoParameterPath;
using WhoParameterClassNetwork = NexusForever.Network.World.Message.Model.Who.Parameter.WhoParameterClass;
using WhoParameterClassInternal = NexusForever.Network.Internal.Message.Who.Parameter.WhoParameterClass;
using WhoParameterZoneNetwork = NexusForever.Network.World.Message.Model.Who.Parameter.WhoParameterZone;
using WhoParameterZoneInternal = NexusForever.Network.Internal.Message.Who.Parameter.WhoParameterZone;
using WhoParameterGuildNetwork = NexusForever.Network.World.Message.Model.Who.Parameter.WhoParameterGuild;
using WhoParameterGuildInternal = NexusForever.Network.Internal.Message.Who.Parameter.WhoParameterGuild;
using WhoParameterPlayerNetwork = NexusForever.Network.World.Message.Model.Who.Parameter.WhoParameterPlayer;
using WhoParameterPlayerInternal = NexusForever.Network.Internal.Message.Who.Parameter.WhoParameterPlayer;
using WhoParameterComboNetwork = NexusForever.Network.World.Message.Model.Who.Parameter.WhoParameterCombo;
using WhoParameterComboInternal = NexusForever.Network.Internal.Message.Who.Parameter.WhoParameterCombo;
using WhoParameterFactionNetwork = NexusForever.Network.World.Message.Model.Who.Parameter.WhoParameterFaction;
using WhoParameterFactionInternal = NexusForever.Network.Internal.Message.Who.Parameter.WhoParameterFaction;
using NexusForever.Game.Static.Entity;

namespace NexusForever.WorldServer.Network.Message.Handler.Who
{
    public static class ParameterMappingExtensions
    {
        public static WhoParameterLevelInternal ToInternal(this WhoParameterLevelNetwork parameter)
        {
            return new WhoParameterLevelInternal
            {
                BottomLevel = parameter.BottomLevel,
                TopLevel    = parameter.TopLevel
            };
        }

        public static WhoParameterRaceInternal ToInternal(this WhoParameterRaceNetwork parameter)
        {
            return new WhoParameterRaceInternal
            {
                RaceId = parameter.RaceId
            };
        }

        public static WhoParameterPathInternal ToInternal(this WhoParameterPathNetwork parameter)
        {
            return new WhoParameterPathInternal
            {
                PathId = parameter.PathId
            };
        }

        public static WhoParameterClassInternal ToInternal(this WhoParameterClassNetwork parameter)
        {
            return new WhoParameterClassInternal
            {
                ClassId = parameter.ClassId
            };
        }

        public static WhoParameterZoneInternal ToInternal(this WhoParameterZoneNetwork parameter)
        {
            return new WhoParameterZoneInternal
            {
                WorldZoneId = parameter.WorldZoneId
            };
        }

        public static WhoParameterGuildInternal ToInternal(this WhoParameterGuildNetwork parameter)
        {
            return new WhoParameterGuildInternal
            {
                GuildName = parameter.GuildName
            };
        }

        public static WhoParameterPlayerInternal ToInternal(this WhoParameterPlayerNetwork parameter)
        {
            return new WhoParameterPlayerInternal
            {
                PlayerName = parameter.PlayerName
            };
        }

        public static WhoParameterComboInternal ToInternal(this WhoParameterComboNetwork parameter)
        {
            return new WhoParameterComboInternal
            {
                SearchString = parameter.SearchString,
                RaceId       = parameter.RaceId != Race.None ? parameter.RaceId : null,
                PathId       = parameter.PathId != Game.Static.PlayerPath.Path.None ? parameter.PathId : null,
                ClassId      = parameter.ClassId != Class.None ? parameter.ClassId : null,
                WorldZoneId  = parameter.WorldZoneId != 0 ? parameter.WorldZoneId : null,
                WorldZoneId2 = parameter.WorldZoneId2 != 0 ? parameter.WorldZoneId2 : null
            };
        }

        public static WhoParameterFactionInternal ToInternal(this WhoParameterFactionNetwork parameter)
        {
            return new WhoParameterFactionInternal
            {
                FactionId = parameter.Faction2Id
            };
        }
    }
}
