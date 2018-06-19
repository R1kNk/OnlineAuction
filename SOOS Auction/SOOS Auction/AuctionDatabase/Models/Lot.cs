using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Web;

namespace SOOS_Auction.AuctionDatabase.Models
{
    public class Lot
    {
        [Key]
        public int LotId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string UserId { get; set; }

        public string WinnerId { get; set; }

        [Required]
        public double MinimalPrice { get; set; }

        [Required]
        public double CurrentPrice { get; set; }

        [Required]
        public double MinimalStep { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        
        [Required]
        public int DaysDuration { get; set; }

        public DateTime FinishDate { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string ImagesUrl { get; set; }

        public List<Bid> Bids { get; set; }

        [Required]
        public virtual LotReceiving LotReceiving { get; set; }

        [Required]
        public virtual LotPayment LotPayment { get; set; }

    }
}