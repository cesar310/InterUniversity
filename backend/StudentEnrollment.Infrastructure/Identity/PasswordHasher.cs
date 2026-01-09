using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Infrastructure.Identity;

public sealed class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyPassword(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);

    public string GenerateTemporaryPassword()
    {
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string numberChars = "0123456789";
        const string symbolChars = "!@#$%^&*";
        const string allChars = upperChars + lowerChars + numberChars + symbolChars;

        var random = new Random();
        var password = new char[12];

        // Ensure at least one of each type
        password[0] = upperChars[random.Next(upperChars.Length)];
        password[1] = lowerChars[random.Next(lowerChars.Length)];
        password[2] = numberChars[random.Next(numberChars.Length)];
        password[3] = symbolChars[random.Next(symbolChars.Length)];

        // Fill the rest randomly
        for (int i = 4; i < 12; i++)
        {
            password[i] = allChars[random.Next(allChars.Length)];
        }

        // Shuffle the password
        return new string(password.OrderBy(_ => random.Next()).ToArray());
    }
}
