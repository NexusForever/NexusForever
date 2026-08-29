using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ClientChatWhisper)]
    public class ClientChatWhisper : IReadable
    {
        public string ToName { get; private set; }
        public string ToRealmName { get; private set; }

        public string Message { get; private set; }
        public List<ChatClientFormat> Formats { get; } = [];
        public ushort ChatMessageId { get; set; } // incremented by the client

        public bool IsAccountWhisper { get; set; }

        #region Dependency Injection

        private readonly IServiceProvider serviceProvider;

        public ClientChatWhisper(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        public void Read(GamePacketReader reader)
        {
            ToName      = reader.ReadWideString();
            ToRealmName = reader.ReadWideString();

            Message     = reader.ReadWideString();

            byte formatCount = reader.ReadByte(5u);
            for (int i = 0; i < formatCount; i++)
            {
                var format = serviceProvider.GetService<ChatClientFormat>();
                format.Read(reader);
                Formats.Add(format);
            }

            ChatMessageId    = reader.ReadUShort();
            IsAccountWhisper = reader.ReadBit();
        }
    }
}
