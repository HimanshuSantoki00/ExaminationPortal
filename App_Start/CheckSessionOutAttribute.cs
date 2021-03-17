using System;
using System.Web;
using System.Web.Mvc;

namespace ExaminationPortal.App_Start
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckSessionOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = HttpContext.Current;
            context.Session["UserID"] = context.Session["UserID"] == null ? string.Empty : context.Session["UserID"];

            if (string.IsNullOrWhiteSpace(context.Session["UserID"].ToString()))
            {
                string redirectTo = "~/Login/Logout";
                if (!string.IsNullOrEmpty(context.Request.RawUrl))
                {
                    redirectTo = string.Format("~/Login/Logout?ReturnUrl={0}", HttpUtility.UrlEncode(context.Request.RawUrl));
                    filterContext.Result = new RedirectResult(redirectTo);
                    return;
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}