using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DietPlanning.Web.Startup))]
namespace DietPlanning.Web
{
  public partial class Startup
  {
    public void Configuration(IAppBuilder app)
    {
     
    }
  }
}
