using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DearCoder.Areas.Identity.Pages
{
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public void OnGet()
        {
            ViewData["HeaderText"] = "Dear Coder";
            ViewData["SubheaderText"] = "Tech letters from Kasey";
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
