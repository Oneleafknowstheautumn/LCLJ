using His.Core;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HISWEBLCLJ.Filter
{
    public class RequestFilter : ActionFilterAttribute
    {
        private readonly IRequest _request;

        //Serilog
        public RequestFilter(IRequest request)   //
        {
            _request = request;
        }
        /// <summary>
        /// Action方法调用之前执行
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _request.OnActionExecuting(context);
        }
        /// <summary>
        /// Action 方法调用后，Result 方法调用前执行
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context) { }
        /// <summary>
        /// Result 方法调用前执行
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context) { }
        /// <summary>
        /// Result 方法调用后执行
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            _request.OnResultExecuted(context);
        }
    }
}
