namespace NexusForever.Shared.Cryptography
{
    public class PasswordProvider
    {
        /// <summary>
        /// Returns a random salt and SRP6 password verifier for supplied email and plaintext password.
        /// </summary>
        public static (string salt, string verifier) GenerateSaltAndVerifier(string email, string password)
        {
            byte[] s = RandomProvider.GetBytes(16u);
            byte[] v = Srp6Provider.GenerateVerifier(s, email, password);
            return (s.ToHexString(), v.ToHexString());
        }
    }
}
