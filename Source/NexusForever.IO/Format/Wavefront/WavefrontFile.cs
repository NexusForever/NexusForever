namespace NexusForever.IO.Format.Wavefront
{
    public class WavefrontFile : ITextWritable
    {
        public List<WavefrontObject> Objects { get; set; } = [];

        public void Write(StreamWriter writer)
        {
            foreach (WavefrontObject @object in Objects)
                @object.Write(writer);
        }
    }
}
