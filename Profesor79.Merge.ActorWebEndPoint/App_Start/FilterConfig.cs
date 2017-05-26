using System.Web;
using System.Web.Mvc;

namespace Profesor79.Merge.ActorWebEndPoint
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
