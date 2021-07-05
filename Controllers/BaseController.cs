using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DearCoder.Controllers
{
    public abstract class BaseController : Controller
    {
        //Get UserId from Claims
        protected string UserId => HttpContext
            .User?
            .Claims?
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
            .Value ?? Guid.NewGuid().ToString();
    }
}
