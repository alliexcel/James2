using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(James.Startup))]
namespace James
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
