using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOOS_Auction.AuctionDatabase.Models
{
    public class UserReview
    {
        public int UserReviewId { get; set; }
        public string UserId { get; set; }
        public string UserIdFrom { get; set; }
        public string Text { get; set; }
        public string Review { get; set; }
        public bool isPositive { get; set; }
        public bool isNegative { get; set; }
        public DateTime date { get; set; }
        public string State { get; set; }


    }
}