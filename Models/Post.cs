using DearCoder.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DearCoder.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Title { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Abstract { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }
        public string Slug { get; set; }
        // Give myself the ability to record the state of any particular Post
        [Display(Name = "Publish State")]
        public PublishState PublishState { get; set; }

        //Navigational properties
        public virtual Blog Blog { get; set; }
    }
}
