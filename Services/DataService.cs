using DearCoder.Data;
using DearCoder.Enums;
using DearCoder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearCoder.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IFileService _fileService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IConfiguration _configuration;

        public DataService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, IFileService fileService, UserManager<BlogUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _fileService = fileService;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task ManageDataAsync()
        {

            //Task 0: Make sure the DB is present by running through the migrations
            await _context.Database.MigrateAsync();

            //Task 1: Seed Roles - Creating Roles and entering them into the system (AspNetRoles)
            await SeedRolesAsync();

            //Task 2: Seed a few users in the system (AspNetUsers)
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            //Are there any roles already in the system?
            if (_context.Roles.Any()) 
                return;

            //Spin through an enum and do stuff
            foreach(var stringRole in Enum.GetNames(typeof(BlogRole)))
            {
                var identityRole = new IdentityRole(stringRole);
                //Create a Role in the system for each role
                await _roleManager.CreateAsync(identityRole);
            }
        }

        private async Task SeedUsersAsync()
        {
            if (_context.Users.Any())
            {
                return;
            }

            var adminUser  = new BlogUser()
            {
                Email = "wahl.kasey@gmail.com",
                UserName = "wahl.Kasey@gmail.com",
                GivenName = "Kasey",
                SurName = "Wahl",
                PhoneNumber = "555-5555",
                EmailConfirmed = true,
                ImageData = await _fileService.EncodeFileAsync("kasey_cropped.png"),
                ContentType = "png"
            };

            var modUser = new BlogUser()
            {
                Email = "yogi_bear@gmail.com",
                UserName = "yogi_bear@gmail.com",
                GivenName = "Yogi",
                SurName = "Bear",
                PhoneNumber = "444-4444",
                EmailConfirmed = true,
                ImageData = await _fileService.EncodeFileAsync("yogi.jpg"),
                ContentType = "jpg"
            };

            //await _userManager.CreateAsync(adminUser, "Abc123!");
            //Only creates the user, doesn't assign user to a role
            await _userManager.CreateAsync(adminUser, _configuration["AdminPassword"]);
            //await _userManager.AddToRoleAsync(adminUser, "Administrator");
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            //Create the mod user (but not the role yet)
            await _userManager.CreateAsync(modUser, _configuration["ModPassword"]);

            //Assign mod to the role
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
        }


    }
}
