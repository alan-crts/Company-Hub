using System.Security.Cryptography;
using System.Text;

namespace CompanyHub.Services;

public class HashService : IHashService
{
    public string GetHash(string input)
    {
        HashAlgorithm hashAlgorithm = SHA512.Create();

        var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

        var sBuilder = new StringBuilder();

        for (var i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString("x2"));

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }
}