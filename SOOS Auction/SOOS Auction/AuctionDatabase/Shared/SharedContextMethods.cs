
using SOOS_Auction.AuctionDatabase;
using SOOS_Auction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOOS_Auction.AuctionDatabase.Shared
{
    static public class SharedContextMethods
    {
        //Data checks
        /// <summary>
        /// Checks if whole database contains atleast one row of data in any table
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static public bool isDatabaseEmpty(this AuctionContext context)
        {
            if (!context.Database.Exists()) { context.Database.Create(); return true; }
            if (isLotsTableEmpty(context))
                return true;
            return false;
        }
        //
        /// <summary>
        /// is Areas table contains data
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static public bool isLotsTableEmpty(AuctionContext context)
        {
            if (!context.Database.Exists()) { context.Database.Create(); return true; }
            if (context.Lots.ToList().Count == 0) return true;
            return false;
        }
    }
}
     