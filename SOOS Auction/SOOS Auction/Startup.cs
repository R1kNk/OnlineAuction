using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SOOS_Auction.Startup))]
namespace SOOS_Auction
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
