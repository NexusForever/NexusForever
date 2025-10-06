using System.Text;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Achievement
{
    // Client connects to Steam and gets achievements for the game to then pass to the game server
    [Message(GameMessageOpcode.ClientSteamAchievements)]
    public class ClientSteamAchievements : IReadable
    {
        public uint SteamGameId { get; private set; } // the Steam game ID for Wildstar is 376570
        public string AchievementData { get; private set; }

        public void Read(GamePacketReader reader)
        {
            SteamGameId = reader.ReadUInt();

            uint length = reader.ReadUInt();
            byte[] bytes = reader.ReadBytes(length);
            AchievementData = Encoding.ASCII.GetString(bytes);
        }
    }
}
