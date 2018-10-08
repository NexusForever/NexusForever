using System.IO;
using System.Text;

namespace NexusForever.StsServer.Network.Packet
{
    public class ClientStsPacket : StsPacket
    {
        public string Method { get; }
        public string Uri { get; }

        public ClientStsPacket(byte[] data)
        {
            using (var reader = new StringReader(Encoding.UTF8.GetString(data)))
            {
                string requestLine = reader.ReadLine();
                if (requestLine == null)
                    throw new InvalidDataException("STS packet contains invalid request line!");

                string[] requestParameters = requestLine.Split(' ');
                if (requestParameters.Length != 3)
                    throw new InvalidDataException("STS packet contains invalid request line!");

                Method   = requestParameters[0];
                Uri      = requestParameters[1];
                Protocol = requestParameters[2];

                while (true)
                {
                    string headerLine = reader.ReadLine();
                    if (headerLine == null)
                        throw new InvalidDataException("STS packet contains an invalid header line!");

                    // empty line between header and body data
                    if (headerLine == string.Empty)
                        break;

                    string[] headerParameters = headerLine.Split(':');
                    if (headerParameters.Length != 2)
                        throw new InvalidDataException("STS packet contains an invalid header line!");

                    if (Headers.ContainsKey(headerParameters[0]))
                        throw new InvalidDataException("STS packet contains duplicate header!");

                    Headers.Add(headerParameters[0], headerParameters[1]);
                }

                if (!Headers.ContainsKey("l"))
                    throw new InvalidDataException("STS packet doesn't contain a length header!");
            }
        }

        public void SetBody(byte[] data, uint length)
        {
            Body = Encoding.UTF8.GetString(data, 0, (int)length);
        }
    }
}
