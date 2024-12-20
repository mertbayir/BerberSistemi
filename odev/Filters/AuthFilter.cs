using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;

namespace odev.Filters
{
    public class AuthFilter : IActionFilter
    {
        private readonly string _role;

        public AuthFilter(string role)
        {
            _role = role; // Hangi rolü kontrol etmek istiyorsak, onu alıyoruz
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            var userRole = context.HttpContext.Session.GetString("Role");

            // Rol bilgisi yoksa veya yanlışsa yönlendirme yapılır
            if (userRole == null || userRole != _role)
            {
                context.Result = new RedirectToActionResult("Login", "User", null); // Ana sayfaya yönlendir
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }


    }
}
