using DearCoder.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearCoder.Models
{
    /// <summary>
    /// Represents a comment that is a child of a blog post
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// The primary key of a Comment
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The foreign key of the associated Post
        /// </summary>
        public int PostId { get; set; }
        /// <summary>
        /// The User Id of the Author
        /// </summary>
        public string AuthorId { get; set; }
        /// <summary>
        /// The User Id of the Moderator
        /// </summary>
        public string ModeratorId { get; set; }
        /// <summary>
        /// Represents the content of the comment
        /// </summary>

        public string Body { get; set; }
        /// <summary>
        /// The date the comment was created
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// The date if/when the comment is moderated
        /// </summary>
        public DateTime? Moderated { get; set; }
        /// <summary>
        /// The edited body of the comment
        /// </summary>
        public string ModeratedBody { get; set; }
        /// <summary>
        /// The category for which the comment was edited
        /// </summary>
        public ModerationType ModerationType {get; set;}
        /// <summary>
        /// The Post that the comment is affiliated with
        /// </summary>
        public virtual Post Post { get; set; }
        /// <summary>
        /// The navigational property for the Author
        /// </summary>
        public virtual BlogUser Author { get; set; }
        /// <summary>
        /// The navigational property for the Moderator
        /// </summary>
        public virtual BlogUser Moderator { get; set; }
    }
}
