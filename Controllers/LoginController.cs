using ExaminationPortal.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace ExaminationPortal.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View("~/views/login/login.cshtml", new User());
        }

        [HttpPost]
        public ActionResult Login(User model)
        {
            try
            {
                DataTable dtLogin = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetUserForLogin_Get", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserName", model.UserName);
                        cmd.Parameters.AddWithValue("@UserPassword", model.UserPassword);

                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtLogin);
                        }
                    }
                }

                if (dtLogin != null && dtLogin.Rows.Count > 0)
                {
                    Session["UserID"] = dtLogin.Rows[0]["UserID"].ToString();
                    Session["UserName"] = dtLogin.Rows[0]["UserName"].ToString();
                    Session["UserType"] = dtLogin.Rows[0]["UserType"].ToString();
                    if (Session["UserType"].ToString() == "2")
                        return RedirectToAction("Index", "Questions");
                    else
                        return RedirectToAction("AssignedPaper", "StudentExam");
                    
                }
                else
                    return Login();
            }
            catch (Exception ex)
            {
                return Login();
            }
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            Session.Remove("UserID");
            Session.Remove("UserName");
            Session.Remove("UserType");
            Session.RemoveAll();

            return RedirectToAction("login", "login");
        }
    }
}