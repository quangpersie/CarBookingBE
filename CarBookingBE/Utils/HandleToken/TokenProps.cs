using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarBookingTest.Utils
{
    public class TokenProps
    {
        public string secretKey = "carbookinggnikoobraccarbookingteamtwoowtmaetfaskjdfsdfkjasdfkjsdkjfdkjajue";
        public string issuer = "carbookingissuer";
        public string audience = "carbookingaudience";
        public int expirationMinutes = 5;
        public string GenerateJwtToken(string secretKey, string issuer, string audience, int expirationMinutes, Claim[] claims)
        {
            // Create token credentials
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // Create token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = signingCredentials
            };

            // Create token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Generate token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Serialize token to string
            return tokenHandler.WriteToken(token);
        }
        public bool VerifyJwtToken(string token, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), // Convert the secret key to bytes
                ValidateIssuer = false, // Set to true if you want to validate the issuer
                ValidateAudience = false, // Set to true if you want to validate the audience
                ClockSkew = TimeSpan.Zero // Set the tolerance for the token's expiration time
            };

            try
            {
                SecurityToken validatedToken;
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                // Token signature is valid
                return true;
            }
            catch
            {
                // Token signature validation failed
                return false;
            }
        }
    }
}