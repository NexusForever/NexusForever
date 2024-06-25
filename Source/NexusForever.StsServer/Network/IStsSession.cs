using NexusForever.Cryptography;
using NexusForever.Database.Auth.Model;
using NexusForever.Network.Session;
using NexusForever.Network.Sts;
using NexusForever.Network.Sts.Model;

namespace NexusForever.StsServer.Network
{
    public interface IStsSession : INetworkSession
    {
        AccountModel Account { get; set; }
        SessionState State { get; set; }

        Srp6Provider KeyExchange { get; set; }

        void EnqueueMessageOk(IWritable message);
        void EnqueueMessageError(ServerErrorMessage message);
        void EnqueueMessage(uint statusCode, string status, IWritable message);
        
        void InitialiseEncryption(byte[] key);
    }
}