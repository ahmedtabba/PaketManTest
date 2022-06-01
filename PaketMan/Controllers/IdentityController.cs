using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PaketMan.Contracts;
using PaketMan.Models;
using PaketMan.Models.Api;

namespace PaketMan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : BaseApiController
    {
        private readonly IIdentityService _IdentityService;
        private readonly UserManager<ApplicationUser> _userManager;
        public IdentityController(IIdentityService IdentityService, UserManager<ApplicationUser> userManager)
        {
            _IdentityService = IdentityService;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });

            }

            var authResponse = await _IdentityService.RegisterAsync(model.Email, model.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });

            }

            var authResponse = await _IdentityService.LoginAsync(model.Email, model.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }

        [HttpPost]
        [Route("ResetMyPassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ResetMyPassword([FromBody] MyPasswordReseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });

            }



            var authResponse = await _IdentityService.ResetPasswordAsync(GetCurrentUser.Email, model.Password, GetCurrentUser.Id);

            if (!authResponse.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }

        [HttpPost]
        [Route("ResetPassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordReseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });

            }

            var user = await _userManager.FindByEmailAsync(model.Email);


            var authResponse = await _IdentityService.ResetPasswordAsync(model.Email, model.Password, user.Id);

            if (!authResponse.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }
    }
}
