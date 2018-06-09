using SOOS_Auction.AuctionDatabase.Models;
using SOOS_Auction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOOS_Auction.AuctionDatabase.Shared
{
    public class AuctionDatabaseInitializer : DropCreateIfDatabaseEmpty
    {
        protected override void Seed(AuctionContext context)
        {
            //List<Lot> lots = new List<Lot>() { new Lot { Name = "Мультиварка Bosch", State = "б/у", StartDate = DateTime.Now, FinishDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 2) } };
            //for (int i = 0; i < lots.Count; i++)
            //{
            //    context.Lots.Add(lots[i]);
            //}
            //context.SaveChanges();
        }
    }
}