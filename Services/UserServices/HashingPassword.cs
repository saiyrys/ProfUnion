using Konscious.Security.Cryptography;
using System.Text;

namespace Profunion.Services.UserServices
{ 
    public class HashingPassword
    {
        public (string HashedPassword, byte[] Salt) HashPassword(string password)
        {
            using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                var salt = GenerateSalt();
                hasher.Salt = salt;
                // You can tweak parameters as needed
                hasher.DegreeOfParallelism = 8;
                hasher.MemorySize = 65536; // 64MB
                hasher.Iterations = 10;

                var hashedPassword = Convert.ToBase64String(hasher.GetBytes(32)); // 32 bytes for a secure hash
                
                return (hashedPassword, salt);
            }
        }
        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[16]; // Adjust the size of the salt as needed
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            byte[] salt = Convert.FromBase64String(storedSalt);

            using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(enteredPassword)))
            {
                hasher.Salt = salt;
                // Установите те же параметры, которые вы использовали при хэшировании пароля
                hasher.DegreeOfParallelism = 8;
                hasher.MemorySize = 65536; // 64MB
                hasher.Iterations = 10;

                var computedHash = Convert.ToBase64String(hasher.GetBytes(32));

                return storedHash.Equals(computedHash);
            }
        }
    }
}
