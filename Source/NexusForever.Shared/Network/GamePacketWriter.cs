using System;
using System.IO;
using System.Text;

namespace NexusForever.Shared.Network
{
    public class GamePacketWriter : IDisposable
    {
        private byte bitPosition;
        private byte bitValue;
        private readonly Stream stream;

        public GamePacketWriter(Stream output)
        {
            stream = output;
        }

        public void Dispose()
        {
            stream?.Dispose();
        }

        public void FlushBits()
        {
            if (bitPosition == 0)
                return;

            stream.WriteByte(bitValue);

            bitPosition = 0;
            bitValue = 0;
        }

        public void Write(bool value)
        {
            if (value)
                bitValue |= (byte)(1 << bitPosition);

            bitPosition++;
            if (bitPosition == 8)
                FlushBits();
        }

        private void WriteBits(ulong value, uint bits)
        {
            for (int i = 0; i < bits; i++)
                Write(Convert.ToBoolean((value >> i) & 1));
        }

        public void Write(byte value, uint bits = 8u)
        {
            if (bits > sizeof(byte) * 8)
                throw new ArgumentException();

            WriteBits(value, bits);
        }

        public void Write(ushort value, uint bits = 16u)
        {
            if (bits > sizeof(ushort) * 8)
                throw new ArgumentException();

            WriteBits(value, bits);
        }

        public void Write(uint value, uint bits = 32u)
        {
            if (bits > sizeof(uint) * 8)
                throw new ArgumentException();

            WriteBits(value, bits);
        }

        public void Write(int value, uint bits = 32u)
        {
            if (bits > sizeof(uint) * 8)
                throw new ArgumentException();

            WriteBits((uint)value, bits);
        }

        public void Write(float value, uint bits = 32)
        {
            if (bits > sizeof(float) * 8)
                throw new ArgumentException();

            WriteBits((uint)BitConverter.SingleToInt32Bits(value), bits);
        }

        public void Write(double value, uint bits = 64u)
        {
            if (bits > sizeof(double) * 8)
                throw new ArgumentException();

            WriteBits((ulong)BitConverter.DoubleToInt64Bits(value), bits);
        }

        public void Write(ulong value, uint bits = 64)
        {
            if (bits > sizeof(double) * 8)
                throw new ArgumentException();

            WriteBits(value, bits);
        }

        public void Write(long value, uint bits = 64)
        {
            if (bits > sizeof(double) * 8)
                throw new ArgumentException();

            Write((ulong)value, bits);
        }

        public void Write<T>(T value, uint bits = 64u) where T : Enum, IConvertible
        {
            if (bits > sizeof(ulong) * 8)
                throw new ArgumentException();

            WriteBits(value.ToUInt64(null), bits);
        }

        public void WriteBytes(byte[] data, uint length = 0u)
        {
            if (length != 0 && length != data.Length)
                throw new ArgumentException();

            foreach (byte value in data)
                WriteBits(value, 8);
        }

        public void Write(ulong[] data, uint elements = 0u)
        {
            if (elements != 0 && elements != data.Length)
                throw new ArgumentException();

            foreach (ulong value in data)
                Write(value);
        }

        public void WriteStringWide(string value)
        {
            byte[] data = Encoding.Unicode.GetBytes(value ?? "");
            bool extended = data.Length >> 1 > 0x7F;

            Write(extended);
            Write(data.Length >> 1, extended ? 15u : 7u);
            WriteBytes(data);
        }

        public void WriteStringFixed(string value)
        {
            string str = $"{value ?? ""}\0";
            byte[] data = Encoding.Unicode.GetBytes(str);

            Write(str.Length, 16);
            WriteBytes(data);
        }

        public void WritePackedFloat(float value)
        {
            ushort PackFloat(float unpacked)
            {
                uint v1 = (uint)BitConverter.SingleToInt32Bits(unpacked);
                uint v2 = v1 & 0x7FFFFFFF;
                uint v3 = (v1 >> 16) & 0x8000;

                if ((v1 & 0x7FFFFFFF) < 0x33800000)
                    return (ushort)v3;
                if (v2 <= 0x387FEFFF)
                    return (ushort)(v3 | ((((v1 & 0x7FFFFF | 0x800000u) >> (int)(113 - ((v1 & 0x7FFFFFFFu) >> 23))) + 4096) >> 13));
                if (v2 > 0x47FFEFFF)
                    return (ushort)(v3 | 0x43FF);
                return (ushort)(v3 | ((v2 - 0x37FFF000) >> 13));
            }

            Write(PackFloat(value));
        }
    }
}
