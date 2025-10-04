using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientChatWhisper)]
    public class ClientChatWhisper : IReadable
    {
        public string PlayerName { get; private set; }
        public string RealmName { get; private set; }

        public string Message { get; private set; }
        public List<ChatClientFormat> Formats { get; } = [];
        public ushort ChatMessageId { get; set; }

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
            PlayerName = reader.ReadWideString();
            RealmName  = reader.ReadWideString();

            Message    = reader.ReadWideString();

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
