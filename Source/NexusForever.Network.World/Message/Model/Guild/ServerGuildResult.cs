using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildResult)]
    public class ServerGuildResult : IWritable
    {
        public Identity GuildIdentity { get; set; }

        /// <summary>
        /// If the operation was a rank update, then this value is the index of the rank that was updated.
        /// Otherwise, this can be 1 or 0.
        /// </summary>
        public uint ReferenceId { get; set; } 
        public string TargetName { get; set; } // The name of the target of the operation.  This can be a character name or rank name.
        public GuildResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(ReferenceId);
            writer.WriteStringWide(TargetName);
            writer.Write(Result, 8u);
        }
    }
}
