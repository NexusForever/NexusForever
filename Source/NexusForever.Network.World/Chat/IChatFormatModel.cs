using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Chat
{
    public interface IChatFormatModel : IReadable, IWritable
    {
        ChatFormatType Type { get; }
    }
}
