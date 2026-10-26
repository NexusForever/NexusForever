using Xunit;

namespace NexusForever.Network.Tests
{
    public class GamePacketReaderTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EndOfStreamDoesNotProduceBitsOnRepeatedReads(bool consumeByteFirst)
        {
            using var reader = new GamePacketReader(new MemoryStream(
                consumeByteFirst ? new byte[] { 0xA5 } : Array.Empty<byte>()));
            if (consumeByteFirst)
                Assert.Equal((byte)0xA5, reader.ReadByte());

            // A failed refill must not expose stale bits on the next read attempt.
            for (int i = 0; i < 16; i++)
                Assert.Throws<EndOfStreamException>(() => reader.ReadBit());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void TruncatedIntegerIsRejected(int availableBytes)
        {
            using var reader = new GamePacketReader(new MemoryStream(new byte[availableBytes]));
            Assert.Throws<EndOfStreamException>(() => reader.ReadUInt());
        }

        [Fact]
        public void TruncatedStringIsRejected()
        {
            // Short string header: extended=false, length=3; only one character follows.
            using var reader = new GamePacketReader(new MemoryStream(new byte[] { 0x06, 0x41 }));
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [Fact]
        public void BitFieldsReadAcrossByteBoundaryInWireOrder()
        {
            using var reader = new GamePacketReader(new MemoryStream(new byte[] { 0xA5, 0xD2 }));
            Assert.Equal((byte)5, reader.ReadByte(3));
            Assert.Equal((ushort)84, reader.ReadUShort(9));
            Assert.Equal((byte)13, reader.ReadByte(4));
            Assert.Equal(0u, reader.BytesRemaining);
        }

        [Fact]
        public void ResetBitsSkipsPaddingAndSeekingRestartsReading()
        {
            using var reader = new GamePacketReader(new MemoryStream(new byte[] { 0xFF, 0x42 }));
            Assert.True(reader.ReadBit());
            reader.ResetBits();
            Assert.Equal((byte)0x42, reader.ReadByte());
            Assert.Throws<EndOfStreamException>(() => reader.ReadBit());

            reader.BytePosition = 0;
            Assert.Equal((byte)0xFF, reader.ReadByte());
        }

        [Fact]
        public void ZeroLengthReadsDoNotRequireInput()
        {
            using var reader = new GamePacketReader(new MemoryStream());
            Assert.Empty(reader.ReadBytes(0));
            Assert.Equal(0u, reader.ReadUInt(0));
            Assert.Equal(0u, reader.BytePosition);
        }
    }
}
