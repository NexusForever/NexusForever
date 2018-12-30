using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NexusForever.Shared.Cryptography
{
    public static class Hasher
    {
        public static string Hash(this Stream stream)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return BitConverter.ToString(sha256.ComputeHash(stream))
                    .Replace("-", "")
                    .ToLower();
            }
        }
        public static string HashBytes(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
                return memoryStream.Hash();
        }

        public static string Hash(this string @string, Encoding encoding = null)
        {
            if(encoding == null)
                encoding = Encoding.UTF8;
            return HashBytes(encoding.GetBytes(@string));
        }

        public static string HashFile(string fileName)
        {
            using (FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                return fileStream.Hash();

        }
    }
}
