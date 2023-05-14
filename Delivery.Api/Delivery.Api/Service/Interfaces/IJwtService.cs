using Delivery.Api.Model.Entity;
using System.IdentityModel.Tokens.Jwt;

namespace Delivery.Api.Service.Interfaces
{
    public interface IJwtService
    {
        JwtSecurityToken GetToken(User user);
    }
}
