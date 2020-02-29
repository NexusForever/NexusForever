using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NexusForever.Database.Auth.Model;
using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Network;
using NexusForever.StsServer.Network.Message;
using NexusForever.StsServer.Network.Message.Model;
using NexusForever.StsServer.Network.Packet;

namespace NexusForever.StsServer.Network
{
    public class StsSession : NetworkSession
    {
        public AccountModel Account { get; set; }
        public SessionState State { get; set; }

        public Srp6Provider KeyExchange { get; set; }

        private Arc4Provider clientEncryption;
        private Arc4Provider serverEncryption;
        private Arc4Provider serverNewEncryption;

        private FragmentedStsPacket onDeck;
        private readonly ConcurrentQueue<ClientStsPacket> incomingPackets = new ConcurrentQueue<ClientStsPacket>();
        private readonly Queue<ServerStsPacket> outgoingPackets = new Queue<ServerStsPacket>();

        private uint sequence;

        public void EnqueueMessageOk(IWritable message)
        {
            EnqueueMessage(200, "OK", message);
        }

        public void EnqueueMessageError(ServerErrorMessage message)
        {
            EnqueueMessage(400, "Bad Request", message);
        }

        public void EnqueueMessage(uint statusCode, string status, IWritable message)
        {
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent             = true,
                IndentChars        = "",
                NewLineChars       = "\n",
                Encoding           = Encoding.UTF8
            };

            using (var stringWriter = new StringWriter())
            using (var writer = XmlWriter.Create(stringWriter, settings))
            {
                writer.WriteStartDocument();
                message.Write(writer);
                writer.WriteEndDocument();
                writer.Flush();

                var packet = new ServerStsPacket(statusCode, status, stringWriter.ToString(), sequence, serverEncryption != null);
                outgoingPackets.Enqueue(packet);
            }
        }

        protected override void OnData(byte[] data)
        {
            clientEncryption?.Decrypt(data);

            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                while (stream.Remaining() != 0)
                {
                    // no packet on deck waiting for additional information, new data will be part of a new packet
                    if (onDeck == null)
                        onDeck = new FragmentedStsPacket();

                    onDeck.Populate(reader);
                    if (onDeck.HasHeader && onDeck.HasBody)
                    {
                        incomingPackets.Enqueue(onDeck.GetPacket());
                        onDeck = null;
                    }
                }
            }
        }

        public override void Update(double lastTick)
        {
            // process pending packet queue
            while (incomingPackets.TryDequeue(out ClientStsPacket packet))
                HandlePacket(packet);

            // flush pending packet queue
            while (outgoingPackets.TryDequeue(out ServerStsPacket packet))
                FlushPacket(packet);

            base.Update(lastTick);
        }

        private void HandlePacket(ClientStsPacket packet)
        {
            IReadable message = MessageManager.Instance.GetMessage(packet.Uri);
            if (message == null)
            {
                log.Info($"Received unknown packet {packet.Uri}");
                return;
            }

            MessageHandlerInfo handlerInfo = MessageManager.Instance.GetMessageHandler(packet.Uri);
            if (handlerInfo == null)
            {
                log.Info($"Received unhandled packet {packet.Uri}");
                return;
            }

            /*if (State != handlerInfo.State)
            {
                log.Info($"Received packet with invalid session state {packet.Uri}");
                return;
            }*/

            if (packet.Headers.TryGetValue("s", out string sequenceString))
                uint.TryParse(sequenceString, out sequence);

            log.Trace($"Received packet {packet.Uri}.");

            if (packet.Body != "")
            {
                var doc = new XmlDocument();
                doc.LoadXml(packet.Body);
                message.Read(doc);
            }

            handlerInfo.Delegate.Invoke(this, message);
        }

        private void FlushPacket(ServerStsPacket packet)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(packet.Protocol);
                writer.Write(" ");
                writer.Write(packet.StatusCode);
                writer.Write(" ");
                writer.Write(" ");
                writer.Write(packet.Status);
                writer.Write("\r\n");

                foreach ((string name, string value) in packet.Headers)
                {
                    writer.Write($"{name}:{value}");
                    writer.Write("\r\n");
                }

                writer.Write("\r\n");
                writer.Write(packet.Body);
                writer.Flush();

                byte[] buffer = stream.ToArray();
                if (packet.Encrypt)
                    serverEncryption.Encrypt(buffer);

                SendRaw(buffer);
            }

            if (serverNewEncryption != null)
            {
                serverEncryption = serverNewEncryption;
                serverNewEncryption = null;
            }

            log.Trace($"Sent packet response {packet.StatusCode}, {packet.Status}");
        }

        public void InitialiseEncryption(byte[] key)
        {
            clientEncryption = new Arc4Provider(key);
            serverNewEncryption = new Arc4Provider(key);
            log.Trace($"Initialised RC4, Key: {BitConverter.ToString(key).Replace("-", "")}");
        }
    }
}
