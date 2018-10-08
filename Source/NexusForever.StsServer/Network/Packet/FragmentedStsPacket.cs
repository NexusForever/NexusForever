using System;
using System.IO;
using NexusForever.Shared.Network;

namespace NexusForever.StsServer.Network.Packet
{
    public class FragmentedStsPacket
    {
        public bool HasHeader => packet != null;
        public bool HasBody => packet?.Body != null;

        private readonly byte[] buffer = new byte[2048];
        private uint position;

        private ClientStsPacket packet;
        private uint dataLength;

        public void Populate(BinaryReader reader)
        {
            if (HasHeader)
            {
                uint remaining = reader.BaseStream.Remaining();
                if (remaining < dataLength)
                {
                    // don't have enough data, push entire frame into packet
                    byte[] data = reader.ReadBytes((int)remaining);
                    Buffer.BlockCopy(data, 0, buffer, (int)position, (int)remaining);
                }
                else
                {
                    // enough data, push required frame data into packet
                    byte[] data = reader.ReadBytes((int)dataLength);
                    Buffer.BlockCopy(data, 0, buffer, (int)position, (int)dataLength);

                    packet.SetBody(buffer, dataLength);
                }

                position += remaining;
            }
            else
            {
                while (reader.BaseStream.Remaining() != 0)
                {
                    buffer[position++] = reader.ReadByte();
                    if (position < 4)
                        continue;

                    // end of header is marked by \r\n\r\n
                    if (BitConverter.ToUInt32(buffer, (int)position - 4) == 0x0A0D0A0Du)
                    {
                        packet     = new ClientStsPacket(buffer);
                        position   = 0;
                        dataLength = uint.Parse(packet.Headers["l"]);
                        if (dataLength == 0)
                            packet.SetBody(buffer, 0);

                        break;
                    }
                }
            }
        }

        public ClientStsPacket GetPacket()
        {
            return packet;
        }
    }
}
