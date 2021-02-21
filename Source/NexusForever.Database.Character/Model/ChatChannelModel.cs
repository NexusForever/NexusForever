using System.Collections.Generic;

namespace NexusForever.Database.Character.Model
{
    public class ChatChannelModel
    {
        public ulong Id { get; set; }
        public byte Type { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public ICollection<ChatChannelMemberModel> Members { get; set; } = new List<ChatChannelMemberModel>();
    }
}
