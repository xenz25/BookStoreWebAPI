using BookStore.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace BookStore.API.Services
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> SignUp(SignUp signUp);

        public Task<string> SignIn(SignIn signIn);
    }

    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly SignInManager<ApplicationUsers> _signInManager;
        private readonly IConfiguration _config;

        public AccountRepository(UserManager<ApplicationUsers> userManager, SignInManager<ApplicationUsers> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        public async Task<IdentityResult> SignUp(SignUp signUp)
        {
            var applicationUser = new ApplicationUsers()
            {
                FirstName = signUp.FirstName,
                LastName = signUp.LastName,
                Email = signUp.Email,
                UserName = signUp.Email
            };

            return await _userManager.CreateAsync(applicationUser, signUp.Password);
        }

        public async Task<string> SignIn(SignIn signIn)
        {
            // Authenticate
            var result = await _signInManager.PasswordSignInAsync(signIn.Email, signIn.Password, false, false);

            if (!result.Succeeded)
            {
                return null;
            }

            // create claims
            var claims = new[] {
                new Claim(ClaimTypes.Name, signIn.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // create a signing key
            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidUser"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256Signature)
            );

            // create and return the token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
