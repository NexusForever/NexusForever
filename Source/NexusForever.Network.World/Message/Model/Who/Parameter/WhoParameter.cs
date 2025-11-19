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

            switch (Type)
            {
                case WhoParameterType.Level:
                    Data = new WhoParameterLevel();
                    break;
                case WhoParameterType.Race:
                    Data = new WhoParameterRace();
                    break;
                case WhoParameterType.Path:
                    Data = new WhoParameterPath();
                    break;
                case WhoParameterType.Class:
                    Data = new WhoParameterClass();
                    break;
                case WhoParameterType.Zone:
                    Data = new WhoParameterZone();
                    break;
                case WhoParameterType.Guild:
                    Data = new WhoParameterGuild();
                    break;
                case WhoParameterType.Player:
                    Data = new WhoParameterPlayer();
                    break;
                case WhoParameterType.Combo:
                    Data = new WhoParameterCombo();
                    break;
                case WhoParameterType.Faction:
                    Data = new WhoParameterFaction();
                    break;
                default:
                    throw new NotImplementedException();
            }

            Data.Read(reader);
        }
    }
}
