using ExaminationPortal.App_Start;
using ExaminationPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace ExaminationPortal.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        [CheckSessionOutAttribute]
        public ActionResult Index()
        {
            try
            {
                DataTable dtResult = new DataTable();

                #region Get User list
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetAllUserList_Get", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtResult);
                        }
                    }
                }
                #endregion

                #region Prepare data to rerurn into view
                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    List<User> list = (from dr in dtResult.AsEnumerable()
                                                    select new User()
                                                    {
                                                        UserID = Convert.ToInt64(dr["UserID"]),
                                                        UserName = Convert.ToString(dr["UserName"]),
                                                        UserPassword = Convert.ToString(dr["UserPassword"]),
                                                        UserType = Convert.ToInt32(dr["UserType"])
                                                    }).ToList();

                    return View("~/views/User/index.cshtml", list);
                }
                else
                    return View("~/views/User/index.cshtml", new List<User>());
                #endregion
            }
            catch (Exception ex)
            {
                return View("~/views/User/index.cshtml", new List<User>());
            }
        }

        [HttpGet]
        [CheckSessionOutAttribute]
        public ActionResult Create()
        {
            return View(new User());
        }

        [HttpPost]
        [CheckSessionOutAttribute]
        public ActionResult Create(User model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    #region Check User name Exists or Not
                    DataTable dt = new DataTable();
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_CheckUsernameExists_GetByName", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@UserName", model.UserName.Trim());
                            using (SqlDataAdapter da = new SqlDataAdapter())
                            {
                                da.SelectCommand = cmd;
                                da.Fill(dt);
                            }
                        }
                    }
                    if (dt.Rows.Count > 0)
                        throw new Exception("User is already exists!!!");
                    #endregion

                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("usp_InsertNewUser_Create", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@UserName", model.UserName.Trim());
                            cmd.Parameters.AddWithValue("@UserPassword", model.UserPassword.Trim());
                            cmd.Parameters.AddWithValue("@UserType", model.IsTeacher ? 2 : 1);
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [CheckSessionOutAttribute]
        public ActionResult Edit(int id)
        {
            var model = new User();
            try
            {
                DataTable dtResult = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetUserRecord_GetByID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", id);
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtResult);
                        }
                    }
                }

                if (dtResult.Rows.Count > 0)
                {
                    model.UserID = Convert.ToInt64(dtResult.Rows[0]["UserID"]);
                    model.UserName = dtResult.Rows[0]["UserName"].ToString();
                    model.UserPassword = dtResult.Rows[0]["UserPassword"].ToString();
                    model.UserType = Convert.ToInt32(dtResult.Rows[0]["UserType"]);
                    model.IsTeacher = model.UserType == 2 ? true: false;
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [CheckSessionOutAttribute]
        public ActionResult Edit(int id, User model)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("UserID is invalid.");

                if (ModelState.IsValid)
                {
                    #region Check User name Exists or Not while updating
                    DataTable dt = new DataTable();
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_CheckUsernameExistsWhileUpdate_GetByName", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@UserID", id);
                            cmd.Parameters.AddWithValue("@UserName", model.UserName);
                            using (SqlDataAdapter da = new SqlDataAdapter())
                            {
                                da.SelectCommand = cmd;
                                da.Fill(dt);
                            }
                        }
                    }
                    if (dt.Rows.Count > 0)
                        throw new Exception("User is already exists!!!");
                    #endregion

                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("usp_UpdateUser_Update", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@UserID", id);
                            cmd.Parameters.AddWithValue("@UserName", model.UserName);
                            cmd.Parameters.AddWithValue("@UserPassword", model.UserPassword);
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
    }
}