using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    // <summary> Sends all of the GuildIds to which a unit belongs.
    // Circles, Warparties, Arena teams, and Communities all work the same on the client/server as normal Guilds so this
    // list will contain any and all of the GuildIds to which the unit belongs. </summary>
    [Message(GameMessageOpcode.ServerUnitMemberOfGuildChange)]
    public class ServerUnitMemberOfGuildChange : IWritable
    {
        public uint UnitId { get; set; }
        public List<ulong> GuildIds { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(GuildIds.Count);
            foreach (var guildId in GuildIds)
            {
                writer.Write(guildId);
            }
        }
    }
}
