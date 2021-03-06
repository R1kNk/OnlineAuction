﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SOOS_Auction.AuctionDatabase.Models;

namespace SOOS_Auction.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim(ClaimTypes.Gender, Gender));
            userIdentity.AddClaim(new Claim("TelephoneNumber", Gender));
            double bal = Balance - BusyBalance;
            userIdentity.AddClaim(new Claim("UnBusyBalance", UnBusyBalance));

            // Add custom user claims here
            return userIdentity;
        }

        [Required]
        public string Gender { get; set; }
        public string TelephoneNumber { get; set; }
        public string AvatarUrl { get; set; }
        public int PositiveReview { get; set; }
        public int NegativeReview { get; set; }
        public string UserLocation { get; set; }
        public double Balance { get; set; }
        public double BusyBalance { get; set; }
        public string UnBusyBalance { get => Math.Round((Balance - BusyBalance),2).ToString(); }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}