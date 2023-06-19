using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GmeRecomendationSystem.Controllers.Filters
{
    public class SingleActionAttribute : ActionFilterAttribute
    {
        private static int isExecuting = 0;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Interlocked.CompareExchange(ref isExecuting, 1, 0) == 0)
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            filterContext.Result = new StatusCodeResult(204);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            Interlocked.Exchange(ref isExecuting, 0);
        }
    }
}
