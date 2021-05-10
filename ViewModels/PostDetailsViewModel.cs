using DearCoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearCoder.ViewModels
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; }
        public List<Blog> Blogs { get; set; } = new List<Blog>();
    }
}
