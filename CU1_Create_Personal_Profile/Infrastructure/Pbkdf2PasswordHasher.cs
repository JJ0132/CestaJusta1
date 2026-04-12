using System.Security.Cryptography;
using CestaJusta.CU1.CreateProfile.Application;

namespace CestaJusta.CU1.CreateProfile.Infrastructure;

public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;

    public PasswordHashResult Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

        return new PasswordHashResult(
            Hash: Convert.ToBase64String(hash),
            Salt: Convert.ToBase64String(salt));
    }
}