using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SOOS_Auction.AuctionDatabase.Models
{
    public class Section
    {
        [Key]
        public int SectionId { get; set; }

        public string Name { get; set; }

        public List<Category> Categories { get; set; }

    }
}