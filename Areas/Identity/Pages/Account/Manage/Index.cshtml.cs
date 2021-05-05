using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DearCoder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DearCoder.Services;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DearCoder.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<BlogUser> _userManager;
        private readonly SignInManager<BlogUser> _signInManager;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;
        public IndexModel(
            UserManager<BlogUser> userManager,
            SignInManager<BlogUser> signInManager, IFileService fileService, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileService = fileService;
            _configuration = configuration;
        }

        public string Username { get; set; }

        public string CurrentImage { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Custom Image")]
            public IFormFile ImageFile { get; set; }

            //public byte[] ProfilePic { get; set; }
            //public string ContentType { get; set; }
            
            [Required]
            [Display(Name = "Given Name")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
            public string GivenName { get; set; }

            [Required]
            [Display(Name = "Surname")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
            public string SurName { get; set; }

            [Display(Name = "Display Name")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
            public string DisplayName { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(BlogUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            CurrentImage = _fileService.DecodeImage(user.ImageData, user.ContentType);

            Input = new InputModel
            {
                DisplayName = user.DisplayName,
                PhoneNumber = phoneNumber,
                GivenName = user.GivenName,
                SurName = user.SurName,
                //ProfilePic = user.ImageData,
                //ContentType = user.ContentType,


            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {

                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }


            if (Input.GivenName != user.GivenName)
            {
                user.GivenName = Input.GivenName;
                await _userManager.UpdateAsync(user);
            }

            if (Input.SurName != user.SurName)
            {
                user.SurName = Input.SurName;
                await _userManager.UpdateAsync(user);
            }

            //Store the new displayName if it has changed
            if(user.DisplayName != Input.DisplayName)
            {
                //Store the new name
                user.DisplayName = Input.DisplayName;
                await _userManager.UpdateAsync(user);
            }

            if (Input.ImageFile is not null)
            {
                user.ImageData = await _fileService.EncodeFileAsync(Input.ImageFile);
                user.ContentType = _fileService.ContentType(Input.ImageFile);
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
