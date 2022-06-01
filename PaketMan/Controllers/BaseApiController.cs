using Microsoft.AspNetCore.Mvc;
using PaketMan.Extensions;
using PaketMan.Models;

namespace PaketMan.Controllers
{
    public class BaseApiController : ControllerBase
    {
        public BaseApiController()
        {
        }
        public ApplicationUser GetCurrentUser
        {
            get
            {
                return HttpContext.GetCurrentUser();
            }
        }
    }
}