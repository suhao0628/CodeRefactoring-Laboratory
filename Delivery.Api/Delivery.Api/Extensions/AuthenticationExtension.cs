using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Delivery.Api.Extensions
{
    public static class AuthenticationExtension
    {
        public static void AddAuthenticationExt(this WebApplicationBuilder builder)
        {
            var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>();

            builder.Services.AddAuthentication
                (authoption => {
                    authoption.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    authoption.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authoption.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                    authoption.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    ValidateIssuer = true,
                    ValidateLifetime = jwtConfig.ValidateLifetime,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.SigningKey)),
                };
            });
        }
    }
}
