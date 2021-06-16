using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Sockets;
using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Network.Message;
using NexusForever.Shared.Network.Message.Model;
using NexusForever.Shared.Network.Packet;

namespace NexusForever.Shared.Network
{
    public abstract class GameSession : NetworkSession
    {
        /// <summary>
        /// Determines if queued incoming packets can be processed during a world update.
        /// </summary>
        public bool CanProcessPackets { get; set; } = true;

        protected PacketCrypt encryption;

        private Pipe onDeck = new Pipe();
        private uint? nextSize = null;
        private uint remaining = 0;
        private readonly ConcurrentQueue<ClientGamePacket> incomingPackets = new();
        private readonly ConcurrentQueue<ServerGamePacket> outgoingPackets = new();

        /// <summary>
        /// Enqueue <see cref="IWritable"/> to be sent to the client.
        /// </summary>
        public void EnqueueMessage(IWritable message)
        {
            if (!MessageManager.Instance.GetOpcode(message, out GameMessageOpcode opcode))
            {
                log.Warn("Failed to send message with no attribute!");
                return;
            }

            if (opcode != GameMessageOpcode.ServerAuthEncrypted
                && opcode != GameMessageOpcode.ServerRealmEncrypted)
                log.Trace($"Sent packet {opcode}(0x{opcode:X}).");

            var packet = new ServerGamePacket(opcode, message);
            outgoingPackets.Enqueue(packet);
        }

        /// <summary>
        /// Enqueue <see cref="IWritable"/> to be sent encrypted to the client.
        /// </summary>
        public void EnqueueMessageEncrypted(IWritable message)
        {
            if (!MessageManager.Instance.GetOpcode(message, out GameMessageOpcode opcode))
            {
                log.Warn("Failed to send message with no attribute!");
                return;
            }

            using (var stream = new MemoryStream())
            using (var writer = new GamePacketWriter(stream))
            {
                writer.Write(opcode, 16);
                message.Write(writer);
                writer.FlushBits();

                byte[] data      = stream.ToArray();
                byte[] encrypted = encryption.Encrypt(data, data.Length);
                EnqueueMessage(BuildEncryptedMessage(encrypted));
            }

            log.Trace($"Sent packet {opcode}(0x{opcode:X}).");
        }

        [Conditional("DEBUG")]
        public void EnqueueMessageEncrypted(uint opcode, string hex)
        {
            using (var stream = new MemoryStream())
            using (var writer = new GamePacketWriter(stream))
            {
                writer.Write(opcode, 16);
                
                byte[] body = Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray();
                writer.WriteBytes(body);
                
                writer.FlushBits();

                byte[] data      = stream.ToArray();
                byte[] encrypted = encryption.Encrypt(data, data.Length);
                EnqueueMessage(BuildEncryptedMessage(encrypted));
            }
        }

        protected abstract IWritable BuildEncryptedMessage(byte[] data);

        public override void OnAccept(Socket newSocket)
        {
            base.OnAccept(newSocket);

            ulong key = PacketCrypt.GetKeyFromAuthBuildAndMessage();
            encryption = new PacketCrypt(key);
        }

        protected override void OnData(byte[] data)
        {
            onDeck.Writer.WriteAsync(data);
            remaining += (uint)data.Length; // Pipe has no way of checking how much it has remaining, so I'm tracking it manually.
            Stream stream = onDeck.Reader.AsStream();
            GamePacketReader reader = new GamePacketReader(stream);

            bool repeat = true;
            while (repeat)
            {
                repeat = false;
                if (nextSize == null && remaining >= sizeof(uint))
                {
                    nextSize = reader.ReadUInt() - sizeof(uint);
                    remaining -= sizeof(uint);
                    repeat = true;
                }
                if (remaining >= nextSize)
                {
                    remaining -= (uint)nextSize;
                    var packet = new ClientGamePacket(reader.ReadBytes((uint)nextSize));

                    incomingPackets.Enqueue(packet);
                    nextSize = null;
                    repeat = true;
                }
            }
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();

            incomingPackets.Clear();
            outgoingPackets.Clear();
        }

        public override void Update(double lastTick)
        {
            // process pending packet queue
            while (CanProcessPackets && incomingPackets.TryDequeue(out ClientGamePacket packet))
                HandlePacket(packet);

            // flush pending packet queue
            while (outgoingPackets.TryDequeue(out ServerGamePacket packet))
                FlushPacket(packet);

            base.Update(lastTick);
        }

        protected void HandlePacket(ClientGamePacket packet)
        {
            IReadable message = MessageManager.Instance.GetMessage(packet.Opcode);
            if (message == null)
            {
                log.Warn($"Received unknown packet {packet.Opcode:X}");
                return;
            }

            MessageHandlerDelegate handlerInfo = MessageManager.Instance.GetMessageHandler(packet.Opcode);
            if (handlerInfo == null)
            {
                log.Warn($"Received unhandled packet {packet.Opcode}(0x{packet.Opcode:X}).");
                return;
            }

            if (packet.Opcode != GameMessageOpcode.ClientEncrypted
                && packet.Opcode != GameMessageOpcode.ClientPacked
                && packet.Opcode != GameMessageOpcode.ClientPackedWorld
                && packet.Opcode != GameMessageOpcode.ClientEntityCommand)
                log.Trace($"Received packet {packet.Opcode}(0x{packet.Opcode:X}).");

            // FIXME workaround for now. possible performance impact. 
            // ClientPing does not currently work and the session times out after 300s -> this keeps the session alive if -any- client packet is received
            Heartbeat.OnHeartbeat();

            using (var stream = new MemoryStream(packet.Data))
            using (var reader = new GamePacketReader(stream))
            {
                message.Read(reader);
                if (reader.BytesRemaining > 0)
                    log.Warn($"Failed to read entire contents of packet {packet.Opcode}");

                try
                {
                    handlerInfo.Invoke(this, message);
                }
                catch (InvalidPacketValueException exception)
                {
                    log.Error(exception);
                    RequestedDisconnect = true;
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                }
            }
        }

        [MessageHandler(GameMessageOpcode.ClientEncrypted)]
        public void HandleEncryptedPacket(ClientEncrypted encrypted)
        {
            byte[] data = encryption.Decrypt(encrypted.Data, encrypted.Data.Length);

            var packet = new ClientGamePacket(data);
            HandlePacket(packet);
        }

        [MessageHandler(GameMessageOpcode.ClientPacked)]
        public void HandlePacked(ClientPacked packed)
        {
            var packet = new ClientGamePacket(packed.Data);
            HandlePacket(packet);
        }

        private void FlushPacket(ServerGamePacket packet)
        {
            using (var stream = new MemoryStream())
            using (var writer = new GamePacketWriter(stream))
            {
                writer.Write(packet.Size);
                writer.Write(packet.Opcode, 16);
                writer.WriteBytes(packet.Data);

                SendRaw(stream.ToArray());
            }
        }
    }
}
