using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PaketMan.Contracts;
using PaketMan.Models;
using PaketMan.Models.Api;
using PaketMan.Models.Api.Identity;
using PaketMan.Extensions;

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

        [HttpGet]
        [Route("/api/Users")]
        public async Task<IActionResult> GetUsers([FromQuery] IdentityRequestParams filter)
        {
            try
            {
                var pagedData =  _userManager.Users;

                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                    pagedData = pagedData.Where(x => x.UserName.Contains(filter.SearchText));


                pagedData = pagedData.OrderBy(filter.Sort);



                pagedData = pagedData.Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize);

                var totalRecords =  _userManager.Users.Count();
                var totalPagees = Math.Ceiling((decimal)totalRecords / filter.PageSize);

                return Ok(new PagedResponse<List<ApplicationUser>>(pagedData.ToList(), filter.PageNumber, filter.PageSize) { TotalRecords = totalRecords, TotalPages = (int)totalPagees });

            }
            catch (Exception ex)
            {
                //log error
                return BadRequest(new FailedResponse
                {
                    Errors = new List<string> { ex.Message }
                });
            }
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
