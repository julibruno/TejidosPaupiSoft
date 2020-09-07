
using System.Web.Optimization;

namespace TejidosPaupiSoft
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

               bundles.Add(new StyleBundle("~/bundles/css")
                .Include(
                   "~/Content/assets/vendor/charts/morris-bundle/morris.css",
                     "~/Content/assets/vendor/charts/chartist-bundle/chartist.css",
                "~/Content/assets/vendor/bootstrap/css/bootstrap.min.css",
                "~/Content/assets/vendor/fonts/circular-std/style.css",
                "~/Content/assets/libs/css/style.css",
                "~/Content/assets/vendor/fonts/fontawesome/css/fontawesome-all.css",
                "~/Content/assets/vendor/datatables/css/dataTables.bootstrap4.css",
                "~/Content/assets/vendor/datatables/css/buttons.bootstrap4.css",
                "~/Content/assets/vendor/datatables/css/select.bootstrap4.css",
                "~/Content/assets/vendor/datatables/css/fixedHeader.bootstrap4.css",
              
                
                "~/Content/assets/vendor/fonts/material-design-iconic-font/css/materialdesignicons.min.css",
                "~/Content/assets/vendor/charts/c3charts/c3.css",
                "~/Content/assets/vendor/fonts/flag-icon-css/flag-icon.min.css"
                
                ));



                 bundles.Add(new StyleBundle("~/bundles/js")
                .Include(
                "~/Content/assets/vendor/jquery/jquery-3.3.1.min.js",
                "~/Content/assets/vendor/bootstrap/js/bootstrap.bundle.js",
                "~/Content/assets/vendor/slimscroll/jquery.slimscroll.js",
                "~/Content/assets/vendor/multi-select/js/jquery.multi-select.js",
                "~/Content/assets/libs/js/main-js.js",
                "~/Content/assets/vendor/jquery/jquery.dataTables.min.js",
                 "~/Content/assets/vendor/datatables/js/dataTables.bootstrap4.min.js",
                  "~/Content/assets/vendor/datatables/js/dataTables.buttons.min.js",
                   "~/Content/assets/vendor/datatables/js/buttons.bootstrap4.min.js",
                 "~/Content/assets/vendor/datatables/js/data-table.js",
                 "~/Content/assets/libs/js/Scripts.js"
                //"~/Content/assets/vendor/Archivos/jszip.min.js",
                //"~/Content/assets/vendor/Archivos/pdfmake.min.js",
                //"~/Content/assets/vendor/Archivos/vfs_fonts.js",
                //"~/Content/assets/vendor/Archivos/buttons.html5.min.js",
                //"~/Content/assets/vendor/Archivos/buttons.print.min.js",
                //"~/Content/assets/vendor/Archivos/buttons.colVis.min.js",
                //"~/Content/assets/vendor/Archivos/dataTables.rowGroup.min.js",
                //"~/Content/assets/vendor/Archivos/dataTables.select.min.js",
                //"~/Content/assets/vendor/Archivos/dataTables.fixedHeader.min.js",
                //"~/Content/assets/vendor/charts/chartist-bundle/chartist.min.js",
                //"~/Content/assets/vendor/charts/sparkline/jquery.sparkline.js",
                //"~/Content/assets/vendor/charts/morris-bundle/raphael.min.js",
                //"~/Content/assets/vendor/charts/morris-bundle/morris.js",
                //"~/Content/assets/vendor/charts/c3charts/c3.min.js",
                //"~/Content/assets/vendor/charts/c3charts/d3-5.4.0.min.js",
                //"~/Content/assets/vendor/charts/c3charts/C3chartjs.js",
                //"~/Content/assets/libs/js/dashboard-ecommerce.js"




                ));
        }
    }
}