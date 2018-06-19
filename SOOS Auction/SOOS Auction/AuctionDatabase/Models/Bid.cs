using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SOOS_Auction.AuctionDatabase.Models
{
    public class Bid
    {
        [Key]
        public int BidId { get; set; }

        [Required]
        public string User { get; set; }

        [Range(0,Double.MaxValue,ErrorMessage ="Цена должна быть больше 0!")]
        public double Price { get; set; }

        [Required]
        public DateTime BidDate { get; set; }

        [Required]
        [ForeignKey("Lot")]
        public int LotId { get; set; }
        public Lot Lot { get; set; }

    }
}