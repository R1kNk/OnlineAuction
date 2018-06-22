using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOOS_Auction.Hubs
{
    public class LotDetailsHub : Hub
    {
        public void UpdateLotData(int id)
        {
            Clients.All.updateLotData(id);
        }



    }
}