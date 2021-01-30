using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace NexusForever.Shared.Cryptography
{
    public sealed class Srp6Provider
    {
        public static readonly BigInteger g = 2;
        public static readonly BigInteger N = new(new byte[] {
            0xE3, 0x06, 0xEB, 0xC0, 0x2F, 0x1D, 0xC6, 0x9F, 0x5B, 0x43, 0x76, 0x83, 0xFE, 0x38, 0x51, 0xFD,
            0x9A, 0xAA, 0x6E, 0x97, 0xF4, 0xCB, 0xD4, 0x2F, 0xC0, 0x6C, 0x72, 0x05, 0x3C, 0xBC, 0xED, 0x68,
            0xEC, 0x57, 0x0E, 0x66, 0x66, 0xF5, 0x29, 0xC5, 0x85, 0x18, 0xCF, 0x7B, 0x29, 0x9B, 0x55, 0x82,
            0x49, 0x5D, 0xB1, 0x69, 0xAD, 0xF4, 0x8E, 0xCE, 0xB6, 0xD6, 0x54, 0x61, 0xB4, 0xD7, 0xC7, 0x5D,
            0xD1, 0xDA, 0x89, 0x60, 0x1D, 0x5C, 0x49, 0x8E, 0xE4, 0x8B, 0xB9, 0x50, 0xE2, 0xD8, 0xD5, 0xE0,
            0xE0, 0xC6, 0x92, 0xD6, 0x13, 0x48, 0x3B, 0x38, 0xD3, 0x81, 0xEA, 0x96, 0x74, 0xDF, 0x74, 0xD6,
            0x76, 0x65, 0x25, 0x9C, 0x4C, 0x31, 0xA2, 0x9E, 0x0B, 0x3C, 0xFF, 0x75, 0x87, 0x61, 0x72, 0x60,
            0xE8, 0xC5, 0x8F, 0xFA, 0x0A, 0xF8, 0x33, 0x9C, 0xD6, 0x8D, 0xB3, 0xAD, 0xB9, 0x0A, 0xAF, 0xEE }, true);

        private byte[] I;
        private BigInteger v;
        private BigInteger s;
        private BigInteger A;
        private BigInteger b;
        private BigInteger B;
        private BigInteger u;
        private BigInteger S;
        private BigInteger M1;
        private BigInteger M2;
        private BigInteger K;

        public static byte[] GenerateVerifier(byte[] s, string I, string p)
        {
            using var sha256 = new SHA256Managed();
            byte[] P = sha256.ComputeHash(Encoding.ASCII.GetBytes($"{I}:{p}"));
            BigInteger x = Hash(true, new BigInteger(s, true), new BigInteger(P, true));
            return BigInteger.ModPow(g, x, N).ToByteArray();
        }

        public Srp6Provider(string I, byte[] s, byte[] v)
        {
            this.I = Encoding.ASCII.GetBytes(I);
            this.s = new BigInteger(s, true);
            this.v = new BigInteger(v, true);
        }

        /// <summary>
        /// Generates the server's credentials that are to be sent to the client.
        /// </summary>
        public byte[] GenerateServerCredentials()
        {
            b = RandomProvider.GetUnsignedBigInteger(128u);
            BigInteger k = Hash(true, N, g);
            B = (k * v + BigInteger.ModPow(g, b, N)) % N;
            return B.ToByteArray(true);
        }

        /// <summary>
        /// Processes the client's credentials. If valid the shared secret is generated and returned.
        /// </summary>
        public void CalculateSecret(byte[] clientA)
        {
            var a = new BigInteger(clientA, true);
            if (a % N == BigInteger.Zero)
                throw new CryptographicException();

            A = a;
            u = Hash(true, A, B);
            S = BigInteger.ModPow(A * BigInteger.ModPow(v, u, N), b, N);
        }

        public byte[] CalculateSessionKey()
        {
            if (S == BigInteger.Zero)
                throw new CryptographicException("Missing data from previous operations: S");

            K = ShaInterleave(S);
            return K.ToByteArray(true);
        }

        /// <summary>
        /// Authenticates the received client evidence message M1 and saves it only if correct.
        /// </summary>
        public bool VerifyClientEvidenceMessage(byte[] clientM1)
        {
            if (A == BigInteger.Zero || B == BigInteger.Zero || S == BigInteger.Zero)
                throw new CryptographicException("Missing data from previous operations: A, B, S");

            using var sha256 = new SHA256Managed();
            var IHash = sha256.ComputeHash(I);
            BigInteger serverM1 = Hash(false, Hash(false, N) ^ Hash(false, g),
                new BigInteger(IHash, true), s, A, B, K);

            if (!clientM1.SequenceEqual(serverM1.ToByteArray(true)))
                return false;

            M1 = new BigInteger(clientM1, true);
            return true;
        }

        /// <summary>
        ///  Computes the server evidence message M2 using the previously verified values.
        /// </summary>
        public byte[] CalculateServerEvidenceMessage()
        {
            if (A == BigInteger.Zero || M1 == BigInteger.Zero || K == BigInteger.Zero)
                throw new CryptographicException("Missing data from previous operations: A, M1, K");

            M2 = Hash(true, A, M1, K);

            byte[] M2Bytes = M2.ToByteArray(true);
            ReverseBytesAsUInt32(M2Bytes);
            return M2Bytes;
        }

        private static BigInteger Hash(bool reverse, params BigInteger[] integers)
        {
            using var sha256 = new SHA256Managed();
            sha256.Initialize();

            for (int i = 0; i < integers.Length; i++)
            {
                byte[] buffer = integers[i].ToByteArray(true);
                int padding = buffer.Length % 4;
                if (padding != 0)
                    Array.Resize(ref buffer, buffer.Length + (4 - padding));

                if (i == integers.Length - 1)
                    sha256.TransformFinalBlock(buffer, 0, buffer.Length);
                else
                    sha256.TransformBlock(buffer, 0, buffer.Length, null, 0);
            }

            byte[] hash = sha256.Hash;
            if (reverse)
                ReverseBytesAsUInt32(hash);
            return new BigInteger(hash, true);
        }

        /// <summary>
        /// Variable length version of SHA_Interleave from RFC2945.
        /// </summary>
        private static BigInteger ShaInterleave(BigInteger key)
        {
            byte[] keyBytes = key.ToByteArray(true);
            byte[] T = keyBytes
                .Reverse()
                .ToArray();

            int first0 = Array.IndexOf<byte>(keyBytes, 0);
            int length = 4;

            if (first0 >= 0 && first0 < T.Length-4)
                length = T.Length - first0;

            byte[] E = new byte[length / 2];
            for (uint i = 0u; i < E.Length; i++)
                E[i] = T[i * 2];

            byte[] F = new byte[length / 2];
            for (uint i = 0u; i < F.Length; i++)
                F[i] = T[i * 2 + 1];

            using var sha256 = new SHA256Managed();
            byte[] G = sha256.ComputeHash(E);
            byte[] H = sha256.ComputeHash(F);

            byte[] K = new byte[G.Length + H.Length];
            for (uint i = 0u; i < K.Length; i++)
            {
                if (i % 2 == 0)
                    K[i] = G[i / 2];
                else
                    K[i] = H[i / 2];
            }

            return new BigInteger(K, true);
        }

        private static void ReverseBytesAsUInt32(byte[] array)
        {
            // works, but thats sloppy.
            int j = array.Length - 4;
            for (int i = 0; i < array.Length / 2; i += 4, j -= 4)
            {
                byte b = array[i + 0];
                array[i + 0] = array[j + 0];
                array[j + 0] = b;
                b = array[i + 1];
                array[i + 1] = array[j + 1];
                array[j + 1] = b;
                b = array[i + 2];
                array[i + 2] = array[j + 2];
                array[j + 2] = b;
                b = array[i + 3];
                array[i + 3] = array[j + 3];
                array[j + 3] = b;
            }
        }
    }
}
