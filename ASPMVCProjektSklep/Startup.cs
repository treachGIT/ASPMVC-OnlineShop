using ASPMVCProjektSklep.Models;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASPMVCProjektSklep.Startup))]
namespace ASPMVCProjektSklep
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            IdentityManager im = new IdentityManager();
            im.CreateRole("Admin");
            im.CreateRole("User");

        }
    }
}
