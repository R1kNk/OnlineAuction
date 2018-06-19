using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SOOS_Auction.AuctionDatabase.Models
{
    public class LotPayment
    {
        [Key]
        public int LotPaymentId { get; set; }

        [Required]
        public bool Cash { get; set; }
        [Required]
        public bool NonCash { get; set; }
        [Required]
        public bool FullPrepaymentPostSending { get; set; }

        public string AdditionalInformation { get; set; }

        public virtual Lot Lot { get; set; }
    }
}