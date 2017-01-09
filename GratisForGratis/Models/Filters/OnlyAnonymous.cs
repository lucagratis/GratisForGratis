using System.Web.Mvc;

namespace GratisForGratis.Filters
{
    public class OnlyAnonymous : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.UrlReferrer.AbsolutePath);
            //filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.DefaultUrl);
            base.OnAuthorization(filterContext);
        }
    }
}
