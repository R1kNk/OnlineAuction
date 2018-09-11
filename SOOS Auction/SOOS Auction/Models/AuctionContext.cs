using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SOOS_Auction.Models
{
    public class AuctionContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public AuctionContext() : base("name=AuctionContext")
        {
        }

        public System.Data.Entity.DbSet<SOOS_Auction.AuctionDatabase.Models.Lot> Lots { get; set; }

        public System.Data.Entity.DbSet<SOOS_Auction.AuctionDatabase.Models.Category> Categories { get; set; }
        public System.Data.Entity.DbSet<SOOS_Auction.AuctionDatabase.Models.Section> Sections { get; set; }
        public System.Data.Entity.DbSet<SOOS_Auction.AuctionDatabase.Models.Bid> Bids { get; set; }
        public System.Data.Entity.DbSet<SOOS_Auction.AuctionDatabase.Models.LotReceiving> LotsReceivings { get; set; }
        public System.Data.Entity.DbSet<SOOS_Auction.AuctionDatabase.Models.LotPayment> LotPayments { get; set; }
        public System.Data.Entity.DbSet<SOOS_Auction.AuctionDatabase.Models.UserReview> Reviews { get; set; }



    }
}
