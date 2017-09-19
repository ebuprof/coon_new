using Forloop.HtmlHelpers;
using System.Web;
using System.Web.Optimization;

namespace coonvey
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/mkitjq").Include(
                       "~/Content/mkit/js/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/Content/mkitjs").Include(
                     "~/Content/mkit/js/material-kit.js",
                      "~/Content/mkit/js/material.min.js" ));

            bundles.Add(new ScriptBundle("~/Content/datepicker").Include(
                     "~/Content/mkit/js/bootstrap-datepicker.js",
                      "~/Content/mkit/js/bootstrap.min.js"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/mdbjs").Include(
                     "~/Content/assets/js/mdb.js",
                     "~/Content/assets/js/mdb.min.js",
                     "~/Content/assets/js/popper.min.js",
                     "~/Content/assets/js/jquery-3.1.1.js",
                    "~/Content/assets/js/jquery-3.1.1.min.js",
                    "~/Content/assets/js/bootstrap.js",
                    "~/Content/assets/js/bootstrap.min.js",
                    "~/Content/assets/js/bootstrap_old.min.js",
                    "~/Content/assets/js/bootstrap_old.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/mdb").Include(
                      "~/Content/assets/css/mdb.css",
                      "~/Content/assets/css/mdb.min.css",
                      "~/Content/assets/css/bootstrap.css",
                      "~/Content/assets/css/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/mkit").Include(
                     "~/Content/mkit/css/bootstrap.min.css",
                     "~/Content/mkit/css/material-kit.css",
                     "~/Content/mkit/css/material-kit.css.map"));

            

            ScriptContext.ScriptPathResolver = System.Web.Optimization.Scripts.Render;
        }
    }
}
