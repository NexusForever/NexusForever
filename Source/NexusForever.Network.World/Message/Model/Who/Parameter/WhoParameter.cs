using NexusForever.Game.Static.Who;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Who.Parameter
{
    public class WhoParameter : IReadable
    {
        public WhoParameterType Type { get; private set; }
        public IWhoParameterData Data { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Type = reader.ReadEnum<WhoParameterType>(4u);

            Data = Type switch
            {
                WhoParameterType.Level   => new WhoParameterLevel(),
                WhoParameterType.Race    => new WhoParameterRace(),
                WhoParameterType.Path    => new WhoParameterPath(),
                WhoParameterType.Class   => new WhoParameterClass(),
                WhoParameterType.Zone    => new WhoParameterZone(),
                WhoParameterType.Guild   => new WhoParameterGuild(),
                WhoParameterType.Player  => new WhoParameterPlayer(),
                WhoParameterType.Combo   => new WhoParameterCombo(),
                WhoParameterType.Faction => new WhoParameterFaction(),
                _                        => throw new NotImplementedException()
            };
            Data.Read(reader);
        }
    }
}
