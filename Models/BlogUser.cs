using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DearCoder.Models
{
    /// <summary>
    /// Represents a user which inherits its properties from IdentityUser
    /// </summary>
    public class BlogUser : IdentityUser
    {
        /// <summary>
        /// The User's given name
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string GivenName { get; set; }
        /// <summary>
        /// The User's family name
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string SurName { get; set; }
        /// <summary>
        /// The User's chosen display name
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// The byte array that stores the User's Profile image data
        /// </summary>

        public byte[] ImageData { get; set; }
        /// <summary>
        /// The extension for the User's profile image
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// The User's Given name and Surname combined
        /// </summary>

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{GivenName} {SurName}";
            }

        }
    }
}
