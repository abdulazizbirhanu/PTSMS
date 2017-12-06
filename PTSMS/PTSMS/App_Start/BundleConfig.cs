using System.Web;
using System.Web.Optimization;

namespace PTSMS
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

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.js"));
          

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/ionicons.css",
                      "~/Content/font-awesome/css/font-awesome.css",
                      "~/Content/AdminLTE/css/AdminLTE.css",
                      "~/Content/AdminLTE/css/skins/_all-skins.css",
                      "~/Content/AdminLTE/plugins/jvectormap/jquery-jvectormap-1.2.2.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/AdminLTE").Include(
                      "~/Content/AdminLTE/js/app.js",                  
                      "~/Content/AdminLTE/plugins/jQuery/jQuery-2.2.0.min.js",
                      "~/Content/AdminLTE/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                      "~/Content/AdminLTE/plugins/jvectormap/jquery-jvectormap-world-mill-en.js",
                      "~/Content/AdminLTE/plugins/daterangepicker/daterangepicker.js",
                      "~/Scripts/bootbox.min.js"
                      ));

             
        }
    }
}
