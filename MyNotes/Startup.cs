using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyNotes.Startup))]
namespace MyNotes
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
