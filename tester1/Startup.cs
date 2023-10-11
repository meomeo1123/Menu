using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(tester1.Startup))]
namespace tester1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
