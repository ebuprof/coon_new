using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(coonvey.Startup))]
namespace coonvey
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
