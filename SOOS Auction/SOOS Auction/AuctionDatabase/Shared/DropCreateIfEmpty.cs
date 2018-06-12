using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using SOOS_Auction.AuctionDatabase.Shared;
using SOOS_Auction.Models;

namespace SOOS_Auction.AuctionDatabase.Shared
{
    public class DropCreateIfDatabaseEmpty : IDatabaseInitializer<AuctionContext>
    {
        public void InitializeDatabase(AuctionContext context)
        {

            if (context.isDatabaseEmpty())
            {
                Seed(context);
            }
        }
        protected virtual void Seed(AuctionContext context)
        {

        }
    }
    public class DropCreateIfIdentityDatabaseEmpty : IDatabaseInitializer<ApplicationDbContext>
    {
        public void InitializeDatabase(ApplicationDbContext context)
        {

            if (context.isIdentityDatabaseEmpty())
            {
                Seed(context);
            }
        }

        protected virtual void Seed(ApplicationDbContext context)
        {

        }
    }
}