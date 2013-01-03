using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace ZCMS
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.6.2.js", "~/Scripts/knockout-2.0.0.js", "~/Scripts/modernizr-2.0.0.js"
                        , "~/Scripts/slimScroll.min.js", "~/Scripts/jquery.colorPicker.js"));

            
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui*", "~/Scripts/timepickeraddon.js"));

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
            "~/Core/Backend/Scripts/FileManager.js"));

            bundles.Add(new ScriptBundle("~/bundles/BackendScriptsSocial").Include(
            "~/Core/Backend/Scripts/Social.js"));

            bundles.Add(new ScriptBundle("~/bundles/BackendScriptsTopics").Include(
            "~/Core/Backend/Scripts/Topics.js"));

            bundles.Add(new ScriptBundle("~/bundles/BackendScriptsSiteDescription").Include(
                "~/Core/Backend/Scripts/SiteDescription.js"));

            bundles.Add(new ScriptBundle("~/bundles/tinymce").Include(
            "~/Scripts/tinymce/jquery.tinymce.js"));

            bundles.Add(new StyleBundle("~/Content/css/backend/main").Include
                ("~/Content/Backend/backend.css", 
                "~/Content/Backend/FileManager.css", 
                "~/Content/Backend/Modal.css",
                "~/Content/Backend/Dashboard.css",
                "~/Content/Backend/Topics.css",
                "~/Content/Backend/ColorPicker.css"));

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

            // Frontend bundle.
            ScriptBundle frontEndBundle = new ScriptBundle("~/bundles/FrontEnd");
            if (System.Web.HttpContext.Current.Application["ActiveSocialMedias"] != null)
            {
                foreach (string s in (List<string>)System.Web.HttpContext.Current.Application["ActiveSocialMedias"])
                {
                    frontEndBundle.Include("~/Scripts/" + s + ".js");
                }
            }
            
            bundles.Add(frontEndBundle);
            bundles.Add(new StyleBundle("~/Content/Frontend/Styles").Include("~/Content/Frontend/*.css"));
        }
    }
}