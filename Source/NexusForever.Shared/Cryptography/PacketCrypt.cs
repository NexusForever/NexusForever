using System;
using System.Linq;

namespace NexusForever.Shared.Cryptography
{
    /// <summary>
    /// Based on the work by Drake Fish with modifications to work with the latest client.
    /// https://bitbucket.org/drakefish/nxsemu/src/master/NxsEmu/Cryptography/PacketCrypt.cs
    /// </summary>
    public class PacketCrypt
    {
        private const int CryptKeyBitSize        = 1024;
        private const int CryptKeySize           = CryptKeyBitSize / 8;
        private const uint CryptMultiplier2      = 0xAA7F8EAAu;
        private const ulong CryptMultiplier      = 0xAA7F8EA9u;
        private const ulong CryptKeyInitialValue = 0x718DA9074F2DEB91u;

        private readonly byte[] key;
        private readonly ulong keyValue;

        public PacketCrypt(ulong keyInteger)
        {
            key = new byte[CryptKeySize];

            ulong keyVal = CryptKeyInitialValue;
            ulong v2 = (keyVal + keyInteger) * CryptMultiplier;
            for (int i = 0; i < CryptKeySize; i += 8)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(v2), 0, key, i, 8);
                keyVal = (keyVal + v2) * CryptMultiplier;
                v2 = (keyInteger + v2) * CryptMultiplier;
            }

            keyValue = keyVal;
        }

        public byte[] Decrypt(byte[] buffer, int length)
        {
            byte[] outputBytes = new byte[length];
            byte[] state = BitConverter.GetBytes(keyValue).Reverse().ToArray();

            uint v4 = CryptMultiplier2 * (uint)length;
            uint v9 = 0;

            for (int i = 0; i < length; i++)
            {
                int stateIndex = i % 8;
                if (stateIndex == 0) // each 8 iteration.
                    v9 = (v4++ & 0xF) * 8;

                byte test = (byte) (state[7 - stateIndex] ^ buffer[i] ^ key[v9 + stateIndex]);
                outputBytes[i] = test;

                // only difference between encrypt and decrypt
                state[7 - stateIndex] = buffer[i];
            }

            return outputBytes;
        }

        public byte[] Encrypt(byte[] buffer, int length)
        {
            byte[] outputBytes = new byte[length];
            byte[] state = BitConverter.GetBytes(keyValue);

            uint v4 = CryptMultiplier2 * (uint)length;
            uint v9 = 0;

            for (int i = 0; i < length; i++)
            {
                int stateIndex = i % 8;
                if (stateIndex == 0) // each 8 iteration.
                    v9 = (v4++ & 0xF) * 8;

                outputBytes[i] = (byte)(state[stateIndex] ^ buffer[i] ^ key[v9 + stateIndex]);

                // only difference between encrypt and decrypt.
                state[stateIndex] = outputBytes[i];
            }

            return outputBytes;
        }

        /// <summary>
        /// Build packet encryption key from client build and auth message.
        /// </summary>
        public static ulong GetKeyFromAuthBuildAndMessage()
        {
            ulong key = CryptKeyInitialValue + 0x5B88D61139619662;
            key = key * CryptMultiplier;
            key = (key + 16042) * CryptMultiplier;
            return (key + 0x97998A0) * CryptMultiplier;

            /*ulong key = CRYPT_KEYVAL_INIT + 0x5B88D61139619662;
            key = key * CRYPT_MULTIPLIER;
            key = (key + 16029) * CRYPT_MULTIPLIER;
            return (key + 0x97998A0) * CRYPT_MULTIPLIER;*/
        }

        /// <summary>
        /// Build packet encryption key from session key.
        /// </summary>
        public static ulong GetKeyFromTicket(byte[] sessionKey)
        {
            if (sessionKey == null)
                throw new ArgumentNullException();
            if (sessionKey.Length != 16)
                throw new ArgumentOutOfRangeException();

            ulong key = CryptKeyInitialValue;
            for (int i = 0; i < 16; i++)
                key = (key + sessionKey[i]) * CryptMultiplier;

            return (key + GetKeyFromAuthBuildAndMessage()) * CryptMultiplier;
        }
    }
}
