using System.Security.Cryptography;
using System.Text;

namespace organization_back_end.Helpers;

public static class Extentions
{
    public static string ToSHA256(this string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashBytes);
    }
}