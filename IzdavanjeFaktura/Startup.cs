using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IzdavanjeFaktura.Startup))]
namespace IzdavanjeFaktura
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
