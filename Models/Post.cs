using DearCoder.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DearCoder.Models
{
    public class Post
    {
        /// <summary>
        /// The Primary Key of the Post
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The Foreign Key to the Blog that is the parent of this Post
        /// </summary>
       
        [Display(Name = "Category")]
        public int BlogId { get; set; }

        /// <summary>
        /// The Title of the Post
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Title { get; set; }
        /// <summary>
        /// A brief subheading expounding on the title
        /// </summary>
        [Display(Name = "Subheading")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Abstract { get; set; }
        /// <summary>
        /// This is the body of the Post.
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// The date on which the Post was created
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }
        /// <summary>
        /// If the Post has been updated, this date reflects the date it was updated
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }
        /// <summary>
        /// This is an internal property used to route to the Details of the Post
        /// </summary>
        public string Slug { get; set; }
        /// <summary>
        /// The state of publication used to determine whether the Post is ready for publication and displays
        /// </summary>
        // Give myself the ability to record the state of any particular Post
        [Display(Name = "Publish State")]
        public PublishState PublishState { get; set; }
        /// <summary>
        /// The byte array for the Image
        /// </summary>
        public byte[] ImageData { get; set; }
        /// <summary>
        /// The file extension for the image
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// The user-input file
        /// </summary>
        [NotMapped]
        [Display(Name = "Add Post Image")]
        public IFormFile ImageFile { get; set; }
        /// <summary>
        /// The navigational property pointing to the parent Blog
        /// </summary>
        //Navigational properties
        public virtual Blog Blog { get; set; }
        /// <summary>
        /// The children of the Post
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
