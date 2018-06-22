using Microsoft.AspNet.SignalR;
using SOOS_Auction.AuctionDatabase.Models;
using SOOS_Auction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOOS_Auction.Hubs
{
    public class TimerUpdateHub : Hub
    {
        static List<TimeExpirationLot> timers;
        static TimerUpdateHub()
        {
            timers = new List<TimeExpirationLot>();
        }
        static bool isLotTimerExists(int LotId)
        {
            TimeExpirationLot findedTimer = timers.Where(p => p.LotId == LotId).SingleOrDefault();
            if (findedTimer == null) return false;
            return true;
        }
        static TimeExpirationLot GetTimer(int id)
        {
            TimeExpirationLot findedTimer = timers.Where(p => p.LotId == id).SingleOrDefault();
            if (findedTimer == null) return null;
            return findedTimer;
        }
        static void DeleteLot(int id)
        {
            if (!isLotTimerExists(id)) return;
            TimeExpirationLot findedTimer = timers.Where(p => p.LotId == id).SingleOrDefault();
            timers.Remove(findedTimer);
        }
        static bool AddNewLot(int lotId)
        {
            AuctionContext auctionContext = new AuctionContext();
            Lot findedLot = auctionContext.Lots.Where(p => p.LotId == lotId).SingleOrDefault();
            if (findedLot == null) return false;
            if (DateTime.Now >= findedLot.FinishDate) return false;
            timers.Add(new TimeExpirationLot() { LotId = findedLot.LotId, FinishLotDate = findedLot.FinishDate });
            return true;
        }
        public void TimeUpdate(int id)
        {
            string span;
            if (!isLotTimerExists(id))
            {
                if (AddNewLot(id))
                {
                    span = GetTimer(id).GetTimeSpanString();
                    if (span == "over")
                    {
                        DeleteLot(id);
                        Clients.Caller.finishLot(id);
                        return;
                    }
                    else
                    {
                        Clients.Caller.updateTimer(id, span);
                        return;
                    }
                }
                Clients.Caller.finishLot(id);
                return;
            }
            else
            {
                if (isLotTimerExists(id))
                {
                    span = GetTimer(id).GetTimeSpanString();
                    if (span == "over")
                    {
                        DeleteLot(id);
                        Clients.Caller.finishLot(id);
                    }
                    else
                    {
                        Clients.Caller.updateTimer(id, span);
                    }
                }
            }
        }
    }

    public class TimeExpirationLot
    {
        public int LotId { get; set; }
        public DateTime FinishLotDate { get; set; }
        public string GetTimeSpanString()
        {
            if (FinishLotDate <= DateTime.Now) return "over";
            TimeSpan span = FinishLotDate - DateTime.Now;
            string spanString = String.Format("{0} д. {1} ч. {2} м. {3} с.",
             span.Days, span.Hours, span.Minutes, span.Seconds);
            return spanString;
        } 
    }
}