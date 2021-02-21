namespace NexusForever.Database.Character.Model
{
    public class ChatChannelMemberModel
    {
        public ulong Id { get; set; }
        public ulong CharacterId { get; set; }
        public byte Flags { get; set; }

        public ChatChannelModel Channel { get; set; }
    }
}
