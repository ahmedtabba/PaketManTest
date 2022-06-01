using PaketMan.Models;
using System.Security.Claims;

namespace PaketMan.Extensions
{
    public static class GeneralExtensions
    {
        public static ApplicationUser GetCurrentUser(this HttpContext httpContext)
        {
            if (httpContext.User == null)
                return null;
            var ss = httpContext.User.Claims;
            var user = new ApplicationUser();
            user.Id = int.TryParse( httpContext.User.Claims.Single(x => x.Type == "id").Value,out int id)?id:0;
            user.Email = httpContext.User.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
            user.UserName = httpContext.User.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
            return user;
        }
    }
}
