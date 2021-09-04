using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KingdomGarden.MVC.Helper
{
    public class TokenHelper
    {
        private static HttpContextBase Current
        {
            get { return new HttpContextWrapper(HttpContext.Current); }
        }
        public object AccessToken
        {
            get { return Current.Session?["AccessToken"]; }
            set
            {
                if (Current.Session != null)
                {
                    Current.Session["AccessToken"] = value;
                }
            }
        }
    }
}