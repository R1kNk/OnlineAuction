using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SOOS_Auction.AuctionDatabase.Models
{
    public class LotReceiving
    {
        [Key]
        public int LotReceivingId { get; set; }

        [Required]
        public string Location { get; set; }
        [Required]
        public bool ByPost { get; set; }
        [Required]
        public bool DeliveryInPerson { get; set; }
        [Required]
        public bool ByPostToAnotherCountry { get; set; }

        [Required]
        public bool ReturnAfterBuyingIsForbidden { get; set; }
            
        public string AdditionalInformation { get; set; }

        public virtual Lot Lot { get; set; }
    }
}