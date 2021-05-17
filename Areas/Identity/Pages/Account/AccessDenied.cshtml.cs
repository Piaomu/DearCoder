using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DearCoder.Areas.Identity.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        public void OnGet()
        {
            ViewData["HeaderText"] = "Dear Coder";
            ViewData["SubheaderText"] = "Tech letters from Kasey";

        }
    }
}

