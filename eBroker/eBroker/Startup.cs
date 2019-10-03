using Hangfire;
using Owin;

namespace eBroker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
            GlobalConfiguration.Configuration.UseSqlServerStorage("eBrokerageEntities");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
