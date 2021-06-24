using DearCoder.Data;
using DearCoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearCoder.Enums;

namespace DearCoder.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IOrderedQueryable<Post> SearchContent(string searchString)
        {
            //Get an IQueryable that contains all of the Posts
            // in the event that the user does not supply a search string.
            var result = _context.Posts.Where(p => p.PublishState == PublishState.ProductionReady);

            if (!string.IsNullOrEmpty(searchString))
            {


                result = result.Where(p => p.Title.Contains(searchString) ||
                                            p.Abstract.Contains(searchString) ||
                                            p.Content.Contains(searchString) ||
                                            p.Comments.Any(c => c.Body.Contains(searchString) ||
                                                                c.ModeratedBody.Contains(searchString) ||
                                                                c.Author.GivenName.Contains(searchString) ||
                                                                c.Author.SurName.Contains(searchString) ||
                                                                c.Author.Email.Contains(searchString)));
            }
            return result.OrderByDescending(p => p.Created);
        }

    }
}
