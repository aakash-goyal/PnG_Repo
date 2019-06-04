using System.Web;
using System.Web.Mvc;

namespace PnG_Cpe_OCR
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
