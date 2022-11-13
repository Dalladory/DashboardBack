using Compass.Data.Data.Interfaces;
using Compass.Data.Data.Models;
using Compass.Data.Data.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Compass.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly TokenValidationParameters _tokenValidationParameters;
        public JwtService(IConfiguration configuration, TokenValidationParameters tokenValidationParameters, IUserRepository userRepository)
        {
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _userRepository = userRepository;
        }

        public async Task<Tokens> GenerateJwtTokenAsync(AppUser user)
        {

            var roles = await _userRepository.GetRolesAsync(user);
       
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Secret"]);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("Id", user.Id),
                new Claim("Name", user.Name == null ? "" : user.Name),
                new Claim("Surname", user.Surname == null ? "" : user.Surname),
                new Claim("Email", user.Email),
                new Claim("EmailConfirm", user.EmailConfirmed.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, roles[0]),
                new Claim(JwtRegisteredClaimNames.Iss,"AppUser"),
                new Claim(JwtRegisteredClaimNames.Aud,"AppUser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())

            }),
                Expires = DateTime.UtcNow.AddMinutes(1), // 5-10 minutes
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(60),
                IsRevoked = false,
                Token = RandomString(25) + Guid.NewGuid()
            };

            await _userRepository.SaveRefreshTokenAsync(refreshToken);

            var tokens = new Tokens();
            tokens.token = jwtToken;
            tokens.refreshToken = refreshToken;

            return tokens;
        }

        public string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<ServiceResponse> VerifyTokenAsync(TokenRequestVM tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _tokenValidationParameters.ValidateLifetime = true;
                var principal = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return null;
                    }
                }

                // Will get the time stamp in unix time
                var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                // we convert the expiry date from seconds to the date
                var expDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expDate > DateTime.UtcNow)
                {
                    return new ServiceResponse()
                    {
                        Errors = new List<string>() { "We cannot refresh this since the token has not expired" },
                        IsSuccess = false
                    };
                }

                // Check the token we got if its saved in the db
                var storedRefreshToken = await _userRepository.CheckRefreshTokenAsync(tokenRequest.RefreshToken);

                //var storedRefreshToken = await _apiDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedRefreshToken == null)
                {
                    return new ServiceResponse()
                    {
                        Errors = new List<string>() { "refresh token doesnt exist" },
                        IsSuccess = false
                    };
                }

                // Check the date of the saved token if it has expired
                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new ServiceResponse()
                    {
                        Errors = new List<string>() { "Token has expired" },
                        IsSuccess = false
                    };
                }

                // check if the refresh token has been used
                if (storedRefreshToken.IsUsed)
                {
                    return new ServiceResponse()
                    {
                        Errors = new List<string>() { "token has been used" },
                        IsSuccess = false
                    };
                }

                // Check if the token is revoked
                if (storedRefreshToken.IsRevoked)
                {
                    return new ServiceResponse()
                    {
                        Errors = new List<string>() { "token has been revoked" },
                        IsSuccess = false
                    };
                }

                // we are getting here the jwt token id
                var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                // check the id that the recieved token has against the id saved in the db
                if (storedRefreshToken.JwtId != jti)
                {
                    return new ServiceResponse()
                    {
                        Errors = new List<string>() { "the token doenst mateched the saved token" },
                        IsSuccess = false
                    };
                }

                storedRefreshToken.IsUsed = true;
                await _userRepository.UpdateRefreshTokenAsync(storedRefreshToken);

                var dbUser = await _userRepository.GetUserByIdAsync(storedRefreshToken.UserId);

                var tokens = new Tokens();
                tokens = await GenerateJwtTokenAsync(dbUser);
                return
                    new ServiceResponse()
                    {
                        AccessToken = tokens.token,
                        RefreshToken = tokens.refreshToken.Token,
                        IsSuccess = true,
                        Message = "Token successfully updated."
                    };
            }
            catch (Exception ex)
            {
                return
                   new ServiceResponse()
                   {
                       IsSuccess = false,
                       Message = ex.Message
                   };
            }
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }
    }

    public class Tokens
    {
        public string token { get; set; }
        public RefreshToken refreshToken { get; set; }
    }
}