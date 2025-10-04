using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientChat)]
    public class ClientChat : IReadable
    {
        public Channel Channel { get; private set; }
        public string Message { get; private set; }
        public List<ChatClientFormat> Formats { get; } = [];
        public ushort ChatMessageId { get; private set; }

        #region Dependency Injection

        private readonly IServiceProvider serviceProvider;

        public ClientChat(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        public void Read(GamePacketReader reader)
        {
            Channel = new Channel();
            Channel.Read(reader);
            Message = reader.ReadWideString();

            byte formatCount = reader.ReadByte(5u);
            for (int i = 0; i < formatCount; i++)
            {
                var format = serviceProvider.GetService<ChatClientFormat>();
                format.Read(reader);
                Formats.Add(format);
            }

            ChatMessageId = reader.ReadUShort();
        }
    }
}
