using KingdomGarden.MVC.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KingdomGarden.MVC.Attributes
{
    public class AuthenticationAttribute : ActionFilterAttribute
    {
        private readonly TokenHelper _tokenHelper;
        //private bool _isAuthorized;

        public AuthenticationAttribute()
        {
            _tokenHelper = new TokenHelper();
        }
        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        //{
        //    _isAuthorized = base.AuthorizeCore(httpContext);
        //    return _isAuthorized;
        //}
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (_tokenHelper.AccessToken == null)
            {
                filterContext.HttpContext.Response.RedirectToRoute("Index", "Account");
            }
        }
    }
}