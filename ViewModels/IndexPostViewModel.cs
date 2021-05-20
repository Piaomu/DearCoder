using DearCoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DearCoder.ViewModels
{
    public class IndexPostViewModel
    {
        public Post LatestPost { get; set; }
        public IPagedList<Blog> Blogs { get; set; }
    }
}
