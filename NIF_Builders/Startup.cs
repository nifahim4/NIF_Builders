using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NIF_Builders.Startup))]
namespace NIF_Builders
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
