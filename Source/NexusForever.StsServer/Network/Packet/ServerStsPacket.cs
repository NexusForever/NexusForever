namespace NexusForever.StsServer.Network.Packet
{
    public class ServerStsPacket : StsPacket
    {
        public uint StatusCode { get; }
        public string Status { get; }
        public bool Encrypt { get; }

        public ServerStsPacket(uint statusCode, string status, string body, uint sequence, bool encrypt)
        {
            StatusCode = statusCode;
            Status     = status;
            Body       = $"{body}\n";
            Encrypt    = encrypt;
            Protocol   = "STS/1.0";

            Headers.Add("l", Body.Length.ToString());
            Headers.Add("s", $"{sequence}R");
        }
    }
}
