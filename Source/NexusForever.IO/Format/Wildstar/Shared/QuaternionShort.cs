namespace NexusForever.IO.Format.WildStar.Shared
{
    /// <summary>
    /// Quaternion stored as 4 signed 16-bit components, divide each component by 16383.5 to get the unit quaternion.
    /// </summary>
    public readonly record struct QuaternionShort(short X, short Y, short Z, short W);
}
