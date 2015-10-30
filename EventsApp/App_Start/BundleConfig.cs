using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;

namespace EventsApp
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // If using {version}, the bundle is just not rendered...
            //bundles.Add(new ScriptBundle("~/bundles/js/modernizr").Include("~/Scripts/modernizr-{version}.js"));
            //bundles.Add(new ScriptBundle("~/bundles/js/jquery").Include("~/Scripts/jquery-{version}.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/js/modernizr").Include("~/Scripts/modernizr-2.6.2.js"));
            bundles.Add(new ScriptBundle("~/bundles/js/jquery").Include("~/Scripts/jquery-1.10.2.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/js/bootstrap").Include("~/Scripts/bootstrap.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/js/bootstrap-datepicker").Include("~/Scripts/bootstrap-datepicker.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/js/event").Include("~/Scripts/event.js"));
            bundles.Add(new StyleBundle("~/bundles/css/style").Include("~/Content/bootstrap.min.css", "~/Content/bootstrap-datepicker.min.css", "~/Content/Site.css"));
        }
    }
}
