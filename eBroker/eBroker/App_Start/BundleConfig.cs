﻿using System.Web.Optimization;

namespace eBroker
{
    public class BundleConfig
    {
        
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/Scripts/fastclick/fastclick.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.min.js",
                "~/Vendor/bootstrap-datepicker/bootstrap-datepicker.min.js",
                "~/Scripts/lib/adminlte.min.js",
                "~/Scripts/gridmvc.min.js",
                "~/Vendor/select2/select2.full.min.js",
                "~/Scripts/respond.js"));
            

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap/bootstrap.min.css",
                "~/Vendor/bootstrap-datepicker/bootstrap-datepicker.min.css",
                "~/Content/lib/AdminLTE.min.css",
                "~/Content/lib/skin-blue.min.css",
                "~/Content/Gridmvc.css",
                "~/Vendor/select2/select2.min.css",
                "~/Content/theme.css",
                "~/Content/site.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}