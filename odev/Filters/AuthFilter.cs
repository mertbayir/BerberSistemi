using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace odev.Filters
{
    public class AuthFilter : ActionFilterAttribute
    {
        private readonly string _allowedUser;

        public AuthFilter(string allowedUser = null)
        {
            _allowedUser = allowedUser;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var username = context.HttpContext.Session.GetString("Email");

            // Eğer oturum bilgisi yoksa login sayfasına yönlendir
            if (string.IsNullOrEmpty(username))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "User" },
                    { "action", "Login" }
                });
            }

            // Eğer belirli bir kullanıcıya izin veriliyorsa ve bu kullanıcı oturumda değilse
            /*if (!string.IsNullOrEmpty(_allowedUser) && username != _allowedUser)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Account" },
                    { "action", "Unauthorized" } // Yetkisiz erişim için bir sayfa
                });
            }*/

            base.OnActionExecuting(context);
        }
    }
}
