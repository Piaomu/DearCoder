using DearCoder.Data;
using DearCoder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using Microsoft.AspNetCore.Identity;
using DearCoder.ViewModels;

namespace DearCoder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync(int? page)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["HeaderText"] = $"Dear {(await _userManager.GetUserAsync(User)).GivenName}";
            }
            else
            {
                ViewData["HeaderText"] = "Dear Coder";
            }

            ViewData["SubheaderText"] = "Tech letters from Kasey";

            var pageNumber = page ?? 1;
            var pageSize = 5;


            //Load the view with all blog data
            var allBlogs = await _context.Blogs.OrderByDescending(b => b.Created)
                                               .ToPagedListAsync(pageNumber, pageSize);
           
            var viewModel = new IndexPostViewModel() {
                LatestPost = await _context.Posts.OrderByDescending(p => p.Created).FirstOrDefaultAsync(p => p.Created != null),
                Blogs = allBlogs
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
