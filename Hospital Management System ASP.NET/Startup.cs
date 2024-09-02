using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Hospital_Management_System_ASP.NET.Startup))]
namespace Hospital_Management_System_ASP.NET
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
