using NexusForever.Database.Character.Model;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Social;

namespace NexusForever.Game.Guild
{
    public class Circle : GuildChat
    {
        public override uint MaxMembers => 20u;

        /// <summary>
        /// Create a new <see cref="Circle"/> using an existing database model.
        /// </summary>
        public Circle(GuildModel baseModel) 
            : base(baseModel)
        {
            InitialiseChatChannels(ChatChannelType.Society, null);
        }

        /// <summary>
        /// Create a new <see cref="Circle"/> using the supplied parameters.
        /// </summary>
        public Circle(string name, string leaderRankName, string councilRankName, string memberRankName)
            : base(GuildType.Circle, name, leaderRankName, councilRankName, memberRankName)
        {
            InitialiseChatChannels(ChatChannelType.Society, null);
        }
    }
}
