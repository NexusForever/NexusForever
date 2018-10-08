using System;
using System.IO;
using System.Text;

namespace NexusForever.Shared.Network
{
    public class GamePacketReader : IDisposable
    {
        public uint BytePosition
        {
            get => (uint)(stream?.Position ?? 0u);
            set
            {
                stream.Position = value;
                ResetBits();
            }
        }
        public uint BytesRemaining => stream?.Remaining() ?? 0u;

        private byte currentBitPosition;
        private byte currentBitValue;
        private readonly Stream stream;

        public GamePacketReader(Stream input)
        {
            stream = input;
            ResetBits();
        }

        public void Dispose()
        {
            stream?.Dispose();
        }

        public void ResetBits()
        {
            if (currentBitPosition > 7)
                return;

            currentBitPosition = 8;
            currentBitValue = 0;
        }

        public bool ReadBit()
        {
            currentBitPosition++;
            if (currentBitPosition > 7)
            {
                currentBitPosition = 0;
                currentBitValue    = (byte)stream.ReadByte();
            }

            return ((currentBitValue >> currentBitPosition) & 1) != 0;
        }

        private ulong ReadBits(uint bits)
        {
            ulong value = 0ul;
            for (uint i = 0u; i < bits; i++)
                if (ReadBit())
                    value |= 1ul << (int)i;

            return value;
        }

        public byte ReadByte(uint bits = 8u)
        {
            if (bits > sizeof(byte) * 8)
                throw new ArgumentException();

            return (byte)ReadBits(bits);
        }

        public ushort ReadUShort(uint bits = 16u)
        {
            if (bits > sizeof(ushort) * 8)
                throw new ArgumentException();

            return (ushort)ReadBits(bits);
        }

        public short ReadShort(uint bits = 16u)
        {
            if (bits > sizeof(short) * 8)
                throw new ArgumentException();

            return (short)ReadBits(bits);
        }

        public uint ReadUInt(uint bits = 32u)
        {
            if (bits > sizeof(uint) * 8)
                throw new ArgumentException();

            return (uint)ReadBits(bits);
        }

        public int ReadInt(uint bits = 32u)
        {
            if (bits > sizeof(int) * 8)
                throw new ArgumentException();

            return (int)ReadBits(bits);
        }

        public unsafe float ReadSingle(uint bits = 32u)
        {
            if (bits > sizeof(float) * 8)
                throw new ArgumentException();

            ulong value = ReadBits(bits);
            return *(float*)&value;
        }

        public unsafe double ReadDouble(uint bits = 64u)
        {
            if (bits > sizeof(double) * 8)
                throw new ArgumentException();

            ulong value = ReadBits(bits);
            return *(double*)&value;
        }

        public ulong ReadULong(uint bits = 64u)
        {
            if (bits > sizeof(ulong) * 8)
                throw new ArgumentException();

            return ReadBits(bits);
        }

        public T ReadEnum<T>(uint bits = 64u) where T : Enum
        {
            if (bits > sizeof(ulong) * 8)
                throw new ArgumentException();

            return (T)Enum.ToObject(typeof(T), ReadBits(bits));
        }

        public byte[] ReadBytes(uint length)
        {
            byte[] data = new byte[length];
            for (uint i = 0u; i < length; i++)
                data[i] = ReadByte();

            return data;
        }

        public string ReadWideStringFixed()
        {
            ushort length = ReadUShort();
            byte[] data = ReadBytes(length * 2u);
            return Encoding.Unicode.GetString(data, 0, data.Length - 2);
        }

        public string ReadWideString()
        {
            bool extended = ReadBit();
            ushort length = (ushort)(ReadUShort(extended ? 15u : 7u) << 1);

            byte[] data = ReadBytes(length);
            return Encoding.Unicode.GetString(data);
        }
    }
}
