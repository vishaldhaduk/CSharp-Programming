using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCSamples.Startup))]
namespace MVCSamples
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
