using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PaketMan.Contracts;
using PaketMan.Models;
using PaketMan.Models.Api;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PaketMan.Repository
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpContext httpContext;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            this.httpContext = httpContext;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "User with this email does not exists!" }
                    };
                }

                var userHavealidPassword = await _userManager.CheckPasswordAsync(user, password);

                if (!userHavealidPassword)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "User/password combination is wrong!" }
                    };
                }

                return await GenerateAuthenticationResultForUser(user);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(email);

                if (existingUser != null)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "User with this email is already exists!" }
                    };
                }

                var newUser = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                };

                var createdUser = await _userManager.CreateAsync(newUser, password);

                if (!createdUser.Succeeded)
                {
                    return new AuthenticationResult
                    {
                        Errors = createdUser.Errors.Select(x => x.Description).ToList()
                    };
                }

                return await GenerateAuthenticationResultForUser(newUser);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUser(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("AAAidSecurityCode20_WithHappy");

            var claimsIdentity = new List<Claim>()
            {
                new Claim(type:JwtRegisteredClaimNames.Sub,value:user.Email),
                        new Claim(type:JwtRegisteredClaimNames.Jti,value:Guid.NewGuid().ToString()),
                        new Claim(type:JwtRegisteredClaimNames.Email,value:user.Email),
                        new Claim(type:"id",value:user.Id.ToString()),
                        new Claim(type:JwtRegisteredClaimNames.Name,value:user.UserName)
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                claimsIdentity.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimsIdentity),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };
        }

        public async Task<AuthenticationResult> ResetPasswordAsync(string email, string password, int userId)
        {
            ApplicationUser cUser = await _userManager.FindByIdAsync(userId.ToString());
            var token = await _userManager.
          GeneratePasswordResetTokenAsync(cUser);
            await _userManager.ResetPasswordAsync(cUser, token, password);
            return await GenerateAuthenticationResultForUser(cUser);

        }
    }
}
