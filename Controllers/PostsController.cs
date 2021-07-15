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
using Microsoft.Extensions.Configuration;
using DearCoder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using DearCoder.Enums;

namespace DearCoder.Controllers
{
    public class PostsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly BasicSlugService _slugService;
        private readonly SearchService _searchService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public PostsController(ApplicationDbContext context,
                               IFileService fileService, 
                               IConfiguration configuration, 
                               BasicSlugService slugService, 
                               SearchService searchService, 
                               UserManager<BlogUser> userManager,
                               IWebHostEnvironment env)
        {
            _context = context;
            _fileService = fileService;
            _configuration = configuration;
            _slugService = slugService;
            _searchService = searchService;
            _userManager = userManager;
            _env = env;
        }

        public async Task<ActionResult> BlogPostIndex(int id, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 4;

            var blogPosts = await _context.Posts.Where(p => p.BlogId == id && p.PublishState == PublishState.ProductionReady)
                                                            .Include(p => p.Blog)
                                                            .OrderByDescending(b => b.Created)
                                                            .ToPagedListAsync(pageNumber, pageSize);

            if (User.Identity.IsAuthenticated)
            {
                ViewData["HeaderText"] = $"Dear {(await _userManager.GetUserAsync(User)).GivenName}";
            }
            else
            {
                ViewData["HeaderText"] = "Dear Coder";
            }
                ViewData["SubheaderText"] = $"Enjoy these {blogPosts.FirstOrDefault()?.Blog.Name}.";
            
            return View(blogPosts);
        }


        // GET: Posts
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
            var applicationDbContext = _context.Posts.Include(p => p.Blog);
            return View(await applicationDbContext.ToListAsync());
        }

        
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> MyPosts()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["HeaderText"] = $"Dear {(await _userManager.GetUserAsync(User)).GivenName}";
            }
            else
            {
                ViewData["HeaderText"] = "Dear Coder";
            }
            ViewData["SubheaderText"] = "These are your Posts.";

            var posts = await _context.Posts
                                      .Include(p => p.Blog)
                                      .Include(p => p.Views)
                                      .Include(p => p.Comments)
                                      .Include(p => p.Title)
                                      .Include(p => p.Abstract)
                                      .Include(p => p.Content)
                                      .Include(p => p.Created)
                                      .Include(p => p.Updated)
                                      .ToListAsync();

            return View();

                                     
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["HeaderText"] = $"Dear {(await _userManager.GetUserAsync(User)).GivenName}";
            }
            else
            {
                ViewData["HeaderText"] = "Dear Coder";
            }

            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.Views)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            //Only count a View if the user is not me.
            if (!User.IsInRole("Administrator"))
            {
                post.Views.Add(new Models.View { UserId = UserId, Created = DateTime.Now });
            }

            await _context.SaveChangesAsync();

            ViewData["SubheaderText"] = $"Enjoy these {post.Blog.Name}.";

            if (post == null)
            {
                return NotFound();
            }

            var dataVM = new PostDetailsViewModel()
            {
                Post = post,
                Blogs = await _context.Blogs.ToListAsync()
            };

            return View(dataVM);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["HeaderText"] = "Dear Coder";
            ViewData["SubheaderText"] = "Please create a post.";

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            return View();
        }

        //Search Index
        public async Task<IActionResult> SearchIndex(int? page, string searchString)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["HeaderText"] = $"Dear {(await _userManager.GetUserAsync(User)).GivenName}";
            }
            else
            {
                ViewData["HeaderText"] = "Dear Coder";
            }
            ViewData["SubheaderText"] = "Here are your search results.";
            ViewData["SearchString"] = searchString;

            //Step 1: I need a set of results stemming from this search string
            var posts = _searchService.SearchContent(searchString);

            var pageNumber = page ?? 1;
            var pageSize = 4;
            


            return View( await posts.ToPagedListAsync(pageNumber, pageSize));
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,PublishState,ImageFile")] Post post)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["HeaderText"] = $"Dear {(await _userManager.GetUserAsync(User)).GivenName}";
            }
            else
            {
                ViewData["HeaderText"] = "Dear Coder";
            }

            ViewData["SubheaderText"] = "Please create a Post.";
            if (ModelState.IsValid)
            {
                post.Created = DateTime.Now;

                post.ImageData = (await _fileService.EncodeFileAsync(post.ImageFile)) ??
                                  await _fileService.EncodeFileAsync(_configuration["DefaultPostImage"]);

                post.ContentType = post.ImageFile is null ?
                                    _configuration["DefaultPostImage"].Split('.')[1] :
                                    _fileService.ContentType(post.ImageFile);

                //Add the post's slug

                var slug = _slugService.UrlFriendly(post.Title);
                if (!_slugService.IsUnique(slug))
                {
                    //I must now add a Model Error and inform the user of the problem
                    ModelState.AddModelError("Title", "There is an issue with the Title. Please try again.");
                    ModelState.AddModelError("", "Where does this thing show up?");
                    ModelState.AddModelError("", "How about this one?");
                    return View(post);
                }

                post.Slug = slug;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("BlogPostIndex", new { id = post.BlogId });
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Administrator")]
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

            ViewData["SubheaderText"] = "Please edit your post.";


            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Created,Slug,Title,Abstract,Content,PublishState,ImageData,ContentType")] Post post, IFormFile newImageFile)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["HeaderText"] = $"Dear {(await _userManager.GetUserAsync(User)).GivenName}";
            }
            else
            {
                ViewData["HeaderText"] = "Dear Coder";
            }

            ViewData["SubheaderText"] = "Please edit your post.";
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newSlug = _slugService.UrlFriendly(post.Title);
                    //I need to compare the original slug with the current slug
                    if(post.Slug != newSlug)
                    {
                        if (!_slugService.IsUnique(newSlug))
                        {
                            ModelState.AddModelError("Title", "There is an issue with the Title, please try again");
                        }
                        post.Slug = newSlug;
                    }

                    //Change the "Created" Date to reflect the date of publication
                    Post oldpost = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == post.Id);
                    PublishState oldPublishState = oldpost.PublishState;
                    
                    if(post.PublishState != oldPublishState && post.PublishState == PublishState.ProductionReady)
                    {
                        post.Created = DateTime.Now;
                    }

                    if (newImageFile is not null)
                    {
                        post.ImageData = await _fileService.EncodeFileAsync(newImageFile);
                        post.ContentType = _fileService.ContentType(newImageFile);
                    }
                    
                    post.Updated = DateTime.Now;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new {slug = post.Slug });
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Administrator")]
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

            ViewData["SubheaderText"] = "Are you sure you want to delete your post?";

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
