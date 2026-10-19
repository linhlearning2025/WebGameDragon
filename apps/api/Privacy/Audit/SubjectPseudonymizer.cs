using System.Security.Cryptography;
using System.Text;

namespace DragonEnglishKids.Api.Privacy.Audit;

public sealed class SubjectPseudonymizer(string key)
{
    private readonly byte[] _key = Encoding.UTF8.GetBytes(key);

    public string Pseudonymize(Guid childId)
    {
        var hash = HMACSHA256.HashData(_key, childId.ToByteArray());
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
