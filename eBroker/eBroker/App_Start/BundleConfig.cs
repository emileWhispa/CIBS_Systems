using System.Web;
using System.Web.Optimization;

namespace eBroker
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                      "~/Scripts/jquery-{version}.js",
                      "~/Scripts/jquery-ui-{version}.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrapjs").Include(
                   "~/Scripts/bootstrap.min.js",
                   "~/Scripts/moment.js",
                   "~/Scripts/jquery.smartWizard.min.js",
                   "~/Scripts/bootstrap-toggle.min.js",
                   "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/templatejs").Include(
                       "~/Scripts/templates/detect.js",
                       "~/Scripts/templates/fastclick.js",
                       "~/Scripts/templates/jquery.slimscroll.js",
                       "~/Scripts/templates/wow.min.js",
                       "~/Scripts/templates/jquery.nicescroll.js",
                       "~/Scripts/templates/jquery.scrollTo.min.js",
                       "~/Scripts/templates/jquery.core.js",
                        "~/Scripts/templates/template.js"
                       ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/jquery-ui.css",
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/font-awesome.css",
                      "~/Content/template.css",
                      "~/Content/smart_wizard.css",
                      "~/Content/smart_wizard_theme_arrows.css",
                      "~/Content/bootstrap-toggle.min.css",
                      "~/Scripts/basic.css",
                      "~/Scripts/dropzone.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/dropzone").Include(
                        "~/Scripts/dropzone.js"));

        }

    }
}
