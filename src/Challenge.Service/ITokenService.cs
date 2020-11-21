using Challenge.Core.Model;

namespace Challenge.Service
{
    public interface ITokenService
    {
        string GenerateToken(Merchant merchant);
    }
}
