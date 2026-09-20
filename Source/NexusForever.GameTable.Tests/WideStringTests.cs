using System.Text;
using Xunit;

namespace NexusForever.GameTable.Tests;

public class WideStringTests
{
    [Theory]
    [InlineData("")]
    [InlineData("WildStar")]
    [InlineData("Grüße 世界")]
    [InlineData("\U0001F680")]
    [InlineData("A\U0001F680B\U00010437C")]
    public void ReadWideStringReadsUtf16AndLeavesFollowingBytes(string text)
    {
        using var stream = new MemoryStream();
        stream.Write(Encoding.Unicode.GetBytes(text + "\0"));
        stream.WriteByte(0x5A);
        stream.Position = 0;
        // The extension must work regardless of the caller's reader encoding.
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);

        Assert.Equal(text, reader.ReadWideString());
        Assert.Equal(0x5A, reader.ReadByte());
        Assert.Equal(stream.Length, stream.Position);
    }

    [Fact]
    public void StringTableCanReadSupplementaryCharactersAtDifferentOffsets()
    {
        const string first = "A\U0001F680";
        const string second = "\U00010437B";
        using var table = new StringTable(Encoding.Unicode.GetBytes(first + "\0" + second + "\0"));

        Assert.Equal(second, table.GetEntry((uint)Encoding.Unicode.GetByteCount(first + "\0")));
        Assert.Equal(first, table.GetEntry(0));
    }

    [Theory]
    [InlineData(new byte[0])]
    [InlineData(new byte[] { 0x41, 0x00 })]
    [InlineData(new byte[] { 0x41, 0x00, 0x00 })]
    public void ReadWideStringRejectsMissingOrTruncatedTerminator(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream);

        Assert.Throws<EndOfStreamException>(() => reader.ReadWideString());
    }
}
