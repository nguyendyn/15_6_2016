using PM_ASVN.Common;
using System.Web;
using System.Web.Mvc;

namespace PM_ASVN
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
