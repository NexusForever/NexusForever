using System;
using System.IO;
using System.Security.Cryptography;

namespace NexusForever.Shared.Cryptography
{
    public static class FileHasher
    {
        public static string HashFile(string fileName)
        {
            using (var fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sha256 = SHA256.Create())
            {
                return BitConverter.ToString(sha256.ComputeHash(fileStream)).Replace(":", "").Replace("-", "")
                    .ToLower();
            }
        }
    }
}