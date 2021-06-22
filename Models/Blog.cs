using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DearCoder.Models
{
    /// <summary>
    /// A Blog category
    /// </summary>
    public class Blog
    {
        /// <summary>
        /// Primary key of a Blog
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The Blog category title
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Name { get; set; }
        /// <summary>
        /// The description of the Blog category
        /// </summary>
        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Description { get; set; }
        /// <summary>
        /// The date the category was created
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }
       /// <summary>
       /// The date the category was updated
       /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }
        /// <summary>
        /// The byte array that stores the image
        /// </summary>
        public byte[] Image { get; set; }
        /// <summary>
        /// The image extension
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// The set of Posts associated with the Blog category
        /// </summary>
        //Navigational properties
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }
}
