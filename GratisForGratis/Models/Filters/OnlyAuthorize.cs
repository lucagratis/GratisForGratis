using System.Web.Mvc;

namespace GratisForGratis.Filters
{
    public class OnlyAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // se non è autenticato e non ha nessun attributo per essere anonimo
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated
                && !filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                && !filterContext.ActionDescriptor.IsDefined(typeof(OnlyAnonymous), true)
                && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(OnlyAnonymous), true)
                )
            {
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.UrlReferrer.AbsolutePath);
            }/*
            else if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.IsDefined(typeof(OnlyAnonymous), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(OnlyAnonymous), true))
            {
                filterContext.HttpContext.SkipAuthorization = false;
            }*/

            base.OnAuthorization(filterContext);
        }
    }
}
