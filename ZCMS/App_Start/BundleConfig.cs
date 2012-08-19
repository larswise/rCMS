using System.Web;
using System.Web.Optimization;

namespace ZCMS
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.*", "~/Scripts/knockout-2.0.0.js"));

            
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/BackendScriptsCommon").Include(
            "~/Core/Backend/Scripts/backend_mainmenu.js", "~/Core/Backend/Scripts/Modal.js"));

            bundles.Add(new ScriptBundle("~/bundles/BackendScriptsPage").Include(
            "~/Core/Backend/Scripts/PageEditor.js"));

            bundles.Add(new ScriptBundle("~/bundles/BackendScriptsDashboard").Include(
            "~/Core/Backend/Scripts/Dashboard.js"));

            bundles.Add(new ScriptBundle("~/bundles/BackendScriptsFile").Include(
            "~/Core/Backend/Scripts/AdminInit.js"));
                    

            bundles.Add(new ScriptBundle("~/bundles/tinymce").Include(
            "~/Scripts/tinymce/jquery.tinymce.js"));

            bundles.Add(new StyleBundle("~/Content/css/backend/main").Include
                ("~/Content/Backend/backend.css", 
                "~/Content/Backend/FileManager.css", 
                "~/Content/Backend/Modal.css", 
                "~/Content/Backend/Dashboard.css"));

            bundles.Add(new StyleBundle("~/Content/css/backend/fileupload").Include("~/Content/Backend/fileuploader.css"));
            bundles.Add(new StyleBundle("~/Content/css/auth/styles").Include("~/Content/Auth/Auth.css"));


            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}