using System;
using System.IO;
using NexusForever.Database.Auth.Model;
using NexusForever.Shared;
using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game.Events;
using NexusForever.StsServer.Network.Message.Model;

namespace NexusForever.StsServer.Network.Message.Handler
{
    public static class AuthenticationHandler
    {
        [MessageHandler("/Auth/LoginStart", SessionState.Connected)]
        public static void HandleLoginStart(StsSession session, ClientLoginStartMessage loginStart)
        {
            session.EnqueueEvent(new TaskGenericEvent<AccountModel>(DatabaseManager.Instance.AuthDatabase.GetAccountByEmailAsync(loginStart.LoginName),
                account =>
            {
                if (account == null)
                {
                    session.EnqueueMessageError(new ServerErrorMessage((int) ErrorCode.InvalidAccountNameOrPassword));
                    return;
                }

                session.Account = account;

                byte[] s = account.S.ToByteArray();
                byte[] v = account.V.ToByteArray();
                session.KeyExchange = new Srp6Provider(account.Email, s, v);

                byte[] B = session.KeyExchange.GenerateServerCredentials();
                using (MemoryStream stream = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(s.Length);
                    writer.Write(s, 0, s.Length);
                    writer.Write(B.Length);
                    writer.Write(B, 0, B.Length);

                    session.EnqueueMessageOk(new ServerLoginStartMessage
                    {
                        KeyData = Convert.ToBase64String(stream.ToArray())
                    });
                }

                session.State = SessionState.LoginStart;
            }));
        }

        [MessageHandler("/Auth/KeyData", SessionState.LoginStart)]
        public static void HandleKeyData(StsSession session, ClientKeyDataMessage keyData)
        {
            session.KeyExchange.CalculateSecret(keyData.A);

            byte[] key = session.KeyExchange.CalculateSessionKey();
            if (!session.KeyExchange.VerifyClientEvidenceMessage(keyData.M1))
            {
                session.EnqueueMessageError(new ServerErrorMessage((int) ErrorCode.InvalidAccountNameOrPassword));
                return;
            }

            byte[] M2 = session.KeyExchange.CalculateServerEvidenceMessage();

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(M2.Length);
                writer.Write(M2, 0, M2.Length);

                session.EnqueueMessageOk(new ServerKeyDataMessage
                {
                    KeyData = Convert.ToBase64String(stream.ToArray())
                });
            }

            // enqueue new key to be set after next packet flush
            session.InitialiseEncryption(key);
        }

        [MessageHandler("/Auth/LoginFinish", SessionState.None)]
        public static void HandleLoginFinish(StsSession session, ClientLoginFinishMessage loginFinish)
        {
            session.EnqueueMessageOk(new ServerLoginFinishMessage
            {
                LocationId = "",
                UserId     = "",
                UserCenter = 0,
                UserName   = "",
                AccessMask = 1L
            });
        }

        [MessageHandler("/Auth/RequestGameToken", SessionState.None)]
        public static void HandleRequestGameToken(StsSession session, RequestGameTokenMessage requestGameToken)
        {
            Guid guid = RandomProvider.GetGuid();
            session.EnqueueEvent(new TaskEvent(DatabaseManager.Instance.AuthDatabase.UpdateAccountGameToken(session.Account, guid.ToByteArray().ToHexString()),
                () =>
            {
                session.EnqueueMessageOk(new RequestGameTokenResponse
                {
                    Token = guid.ToString()
                });
            }));
        }
    }
}
