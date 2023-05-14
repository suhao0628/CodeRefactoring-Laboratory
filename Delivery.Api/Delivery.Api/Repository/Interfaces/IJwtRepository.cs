using Delivery.Api.Model.Entity;
using System.IdentityModel.Tokens.Jwt;

namespace Delivery.Api.Repository.Interfaces
{
    public interface IJwtRepository
    {
        JwtSecurityToken GetToken(User user);
    }
}
