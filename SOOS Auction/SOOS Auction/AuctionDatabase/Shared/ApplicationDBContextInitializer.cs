using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SOOS_Auction.Models;

namespace SOOS_Auction.AuctionDatabase.Shared
{
    public class ApplicationDBContextInitializer : DropCreateIfIdentityDatabaseEmpty
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole("admin"),
                new IdentityRole("moder"),
                new IdentityRole("user")
            };

            foreach (var role in roles)
                roleManager.Create(role);

            ApplicationUser admin = new ApplicationUser() { Email = "admin@gmail.com", UserName = "admin1", TelephoneNumber="+375447146546", Gender="мужской", UserLocation="Минск" , PositiveReview=12, NegativeReview=2, AvatarUrl= "1R65ppqtbBGs3CJMKRDL8Mb5cA1WpA1-y" };
            string password = "Adminpass_1";
            IdentityResult result = userManager.Create(admin, password);
            if (result.Succeeded)
            {
                userManager.AddToRole(admin.Id, roles[0].Name);
                userManager.AddToRole(admin.Id, roles[1].Name);

            }

            ApplicationUser moderator = new ApplicationUser() { Email = "moder@gmail.com", UserName = "moder1", TelephoneNumber = "+375297146546", Gender = "мужской", UserLocation = "Минск", PositiveReview = 10, NegativeReview = 1, AvatarUrl = "1R65ppqtbBGs3CJMKRDL8Mb5cA1WpA1-y" };
            password = "Moderpass_1";
            result = userManager.Create(moderator, password);
            if (result.Succeeded)
            {
                userManager.AddToRole(moderator.Id, roles[1].Name);
            }

            ApplicationUser user = new ApplicationUser() { Email = "user@gmail.com", UserName = "user1", TelephoneNumber = "+375258886546", Gender = "мужской", UserLocation = "Минск", PositiveReview = 7, NegativeReview = 0, AvatarUrl = "1R65ppqtbBGs3CJMKRDL8Mb5cA1WpA1-y" };
            password = "Userpass_1";
            result = userManager.Create(user, password);
            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id, roles[2].Name);
            }
            //context.SaveChanges();
           base.Seed(context);
        }
    }
}