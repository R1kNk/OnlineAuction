
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
            if (isLotsTableEmpty(context) && isBidsTableEmpty(context) && isLotsReceivingTableEmpty(context) && isCategoriesTableEmpty(context)&& isSectionsTableEmpty(context)&& isReviewsTableEmpty(context))
                return true;
            return false;
        }
        //
        /// <summary>
        /// is Lots table contains data
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static public bool isLotsTableEmpty(AuctionContext context)
        {
            if (!context.Database.Exists()) { context.Database.Create(); return true; }
            if (context.Lots.ToList().Count == 0) return true;
            return false;
        }
        /// <summary>
        /// is Areas table contains data
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static public bool isReviewsTableEmpty(AuctionContext context)
        {
            if (!context.Database.Exists()) { context.Database.Create(); return true; }
            if (context.Reviews.ToList().Count == 0) return true;
            return false;
        }
        /// <summary>
        /// is Bids table contains data
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static public bool isBidsTableEmpty(AuctionContext context)
        {
            if (!context.Database.Exists()) { context.Database.Create(); return true; }
            if (context.Bids.ToList().Count == 0) return true;
            return false;
        }

        /// <summary>
        /// is Lots receiving table contains data
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static public bool isLotsReceivingTableEmpty(AuctionContext context)
        {
            if (!context.Database.Exists()) { context.Database.Create(); return true; }
            if (context.LotsReceivings.ToList().Count == 0) return true;
            return false;
        }

        /// <summary>
        /// is Categories table contains data
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static public bool isCategoriesTableEmpty(AuctionContext context)
        {
            if (!context.Database.Exists()) { context.Database.Create(); return true; }
            if (context.Categories.ToList().Count == 0) return true;
            return false;
        }

        /// <summary>
        /// is Sections table contains data
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static public bool isSectionsTableEmpty(AuctionContext context)
        {
            if (!context.Database.Exists()) { context.Database.Create(); return true; }
            if (context.Sections.ToList().Count == 0) return true;
            return false;
        }

        /// <summary>
        /// Checks is whole identity database is empty
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static public bool isIdentityDatabaseEmpty(this ApplicationDbContext context)
        {
            if (!context.Database.Exists()) { context.Database.Create(); return true; }
            if (isRolesTableEmpty(context) && isUsersTableEmpty(context))
                return true;
            return false;
        }

        /// <summary>
        /// Checks is Identity db contains any roles
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static public bool isRolesTableEmpty(ApplicationDbContext context)
        {
            if (!context.Database.Exists()) { context.Database.Create(); return true; }
            if (context.Roles.ToList().Count == 0) return true;
            return false;
        }

        /// <summary>
        /// Checks is Identity db contains any users
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static public bool isUsersTableEmpty(ApplicationDbContext context)
        {
            if (!context.Database.Exists()) { context.Database.Create(); return true; }
            if (context.Users.ToList().Count == 0) return true;
            return false;
        }
    }
}
     