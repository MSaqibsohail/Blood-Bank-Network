using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace BloodBankNetwork.Controllers
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            // Agar user login cookie rakhta hai lekin server ka session khali ho chuka hai (Tab Close hone par)
            if (context.HttpContext.User.Identity.IsAuthenticated && string.IsNullOrEmpty(session.GetString("UserSessionActive")))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Account" },
                        { "action", "Login" }
                    });
            }

            base.OnActionExecuting(context);
        }
    }
}