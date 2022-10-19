using BookStore.API.Models;
using BookStore.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpPost("SignUp")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> SignUp([FromBody] SignUp signUp)
        {
            var result = await _accountRepository.SignUp(signUp);

            if (result.Succeeded)
            {
                return Ok(result.ToString());
            }

            return BadRequest(new { errors = result.Errors });
        }

        [HttpPost("SignIn")]
        [Consumes("application/json")]
        [Produces("text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignIn([FromBody] SignIn signIn)
        {
            // var item = HttpContext.User.Identity as ClaimsIdentity;

            var header = Request.Headers.ToString();
            Debug.WriteLine(header);

            var result = await _accountRepository.SignIn(signIn);

            if (string.IsNullOrEmpty(result))
            {
                return Unauthorized();
            }

            // One approach is store the token to cookies
            // and return the key for the cookie to the client app this it will use the key to grab the access token
            // and add authorization header to its request
            dynamic token = new { token = result };
            return Ok(JsonConvert.SerializeObject(token));
        }

        [HttpPost("SignInCookie")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignInCookie([FromBody] SignIn signIn)
        {

            // One approach is store the token to cookies
            // and return the key for the cookie to the client app this it will use the key to grab the access token
            // and add authorization header to its request
            // var item = HttpContext.User.Identity as ClaimsIdentity;

            var header = Request.Headers.ToString();
            Debug.WriteLine(header);

            var result = await _accountRepository.SignIn(signIn);

            if (string.IsNullOrEmpty(result))
            {
                return Unauthorized();
            }

            return Ok(new { token = result });
        }

    }
}
