using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Achievements
{
    // Client connects to Steam and gets achievements for the game to then pass to the game server
    [Message(GameMessageOpcode.ClientSteamAchievements)]
    public class ClientSteamAchievements : IReadable
    {
        public uint SteamGameId { get; private set; } // the Steam game ID for Wildstar is 376570
        public uint Length { get; private set; }
        public char[] AchievementData { get; private set; } 

        public void Read(GamePacketReader reader)
        {
            SteamGameId = reader.ReadUInt();
            Length = reader.ReadUInt();

            AchievementData = new char[Length];
            for (int i = 0; i < Length; i++)
            {
                AchievementData[i] = (char)reader.ReadByte();
            }
        }
    }
}
