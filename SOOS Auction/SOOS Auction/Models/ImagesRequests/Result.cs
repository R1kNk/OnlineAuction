using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOOS_Auction.Models.ImagesRequests
{
    public class Result
    {
        public string Error { get; set; }
        public List<string> Files { get; set; }
    }
}