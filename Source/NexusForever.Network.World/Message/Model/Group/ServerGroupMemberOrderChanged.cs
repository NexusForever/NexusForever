using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupMemberOrderChanged)]
    public class ServerGroupMemberOrderChanged : IWritable
    {
        public ulong GroupId { get; set; }
        public uint Unused { get; set; } // Unpacked but unused by Client
        public List<Identity> MemberIdentities { get; set; } = new List<Identity>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(Unused);
            writer.Write(MemberIdentities.Count);
            MemberIdentities.ForEach(x => x.Write(writer));
            foreach(var memberIdentity in MemberIdentities)
            {
                writer.Write(MemberIdentities.IndexOf(memberIdentity)); // TODO: Check the List indexing matches how the client uses it
            }
        }
    }
}
