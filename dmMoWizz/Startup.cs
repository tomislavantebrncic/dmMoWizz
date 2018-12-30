using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(dmMoWizz.Startup))]
namespace dmMoWizz
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
