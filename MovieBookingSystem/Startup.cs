using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MovieBookingSystem.Startup))]
namespace MovieBookingSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
