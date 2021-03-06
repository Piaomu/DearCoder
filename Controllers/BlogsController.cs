using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DearCoder.Data;
using DearCoder.Models;
using DearCoder.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DearCoder.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<BlogUser> _userManager;

        //constructor injection
        public BlogsController(ApplicationDbContext context, IFileService fileService, IConfiguration configuration, UserManager<BlogUser> userManager)
        {
            _context = context;
            _fileService = fileService;
            _configuration = configuration;
            _userManager = userManager;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
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

            return View(await _context.Blogs.ToListAsync());
        }

        // GET: Blogs/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["HeaderText"] = "Dear Coder";
            ViewData["SubheaderText"] = "Please create a blog category.";

            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Blog blog, IFormFile Image)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["HeaderText"] = $"Dear {(await _userManager.GetUserAsync(User)).GivenName}";
            }
            else
            {
                ViewData["HeaderText"] = "Dear Coder";
            }
            ViewData["SubheaderText"] = "Please create a blog category.";

            if (ModelState.IsValid)
            {
                blog.Image = (await _fileService.EncodeFileAsync(Image)) ??
                              await _fileService.EncodeFileAsync(_configuration["DefaultBlogImage"]);

                blog.ContentType = blog.Image is null ?
                                    _configuration["DefaultBlogImage"].Split('.')[1] :
                                    _fileService.ContentType(Image);

                blog.Created = DateTime.Now;
               

                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(blog);
        }
        [Authorize(Roles = "Administrator")]
        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (User.Identity.IsAuthenticated)
            {
                ViewData["HeaderText"] = $"Dear {(await _userManager.GetUserAsync(User)).GivenName}";
            }
            else
            {
                ViewData["HeaderText"] = "Dear Coder";
            }

            ViewData["SubheaderText"] = "Please edit your blog category.";

            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Created,Updated,Image,ContentType")] Blog blog, IFormFile NewImage)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["HeaderText"] = $"Dear {(await _userManager.GetUserAsync(User)).GivenName}";
            }
            else
            {
                ViewData["HeaderText"] = "Dear Coder";
            }
            ViewData["SubheaderText"] = "Please edit your blog category.";

            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    blog.Created = blog.Created;
                    blog.Updated = DateTime.Now;

                    if (NewImage is not null)
                    {
                        blog.ContentType = _fileService.ContentType(NewImage);
                        blog.Image = await _fileService.EncodeFileAsync(NewImage);
                    }

                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }
        [Authorize(Roles = "Administrator")]
        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (User.Identity.IsAuthenticated)
            {
                ViewData["HeaderText"] = $"Dear {(await _userManager.GetUserAsync(User)).GivenName}";
            }
            else
            {
                ViewData["HeaderText"] = "Dear Coder";
            }

            ViewData["SubheaderText"] = "Are you sure you want to delete this blog category?";

            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
