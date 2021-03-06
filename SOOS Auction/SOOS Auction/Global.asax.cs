﻿using SOOS_Auction.AuctionDatabase;
using SOOS_Auction.AuctionDatabase.Shared;
using SOOS_Auction.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SOOS_Auction
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer<AuctionContext>(new AuctionDatabaseInitializer());
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDBContextInitializer());
        }
    }
}
