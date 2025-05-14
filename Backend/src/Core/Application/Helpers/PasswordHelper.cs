using System.Security.Cryptography;

namespace Application.Helpers;

public static class PasswordHelper
{
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 10000; // Number of iterations for PBKDF2

    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

    public static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public static bool VerifyPassword(string password, string storedPasswordHash)
    {
        var parts = storedPasswordHash.Split('-');
        var storedHash = Convert.FromHexString(parts[0]);
        var storedSalt = Convert.FromHexString(parts[1]);

        var inputHash = Rfc2898DeriveBytes.Pbkdf2(password, storedSalt, Iterations, Algorithm, HashSize);

        return CryptographicOperations.FixedTimeEquals(inputHash, storedHash);
    }
}