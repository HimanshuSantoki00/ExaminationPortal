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
    public class PaperAssignmentController : Controller
    {
        // GET: PaperAssignment
        [CheckSessionOutAttribute]
        public ActionResult Index()
        {
            try
            {
                var model = new StudentPaperDetail();
                DataTable dtDropDowntList = new DataTable();
                DataTable dtResult = new DataTable();

                #region Prepare dropdown data
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetAllStudentList_Get", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtDropDowntList);
                        }
                    }
                }
                ViewBag.StudentList = ToSelectList(dtDropDowntList, "UserID", "UserName");

                dtDropDowntList = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetAllQuestionPaperList_Get", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtDropDowntList);
                        }
                    }
                }
                ViewBag.QPaperList = ToSelectList(dtDropDowntList, "QPaperID", "QPaperName");
                #endregion

                #region Get prepare list
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetAllStudentPaperDetailList_Get", con))
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
                    List<StudentPaperDetail> list = (from dr in dtResult.AsEnumerable()
                                                     select new StudentPaperDetail()
                                                     {
                                                         StudentPaperDetailID = Convert.ToInt64(dr["StudentPaperDetailID"]),
                                                         QPaperID = Convert.ToInt64(dr["QPaperID"]),
                                                         UserID = Convert.ToInt64(dr["UserID"]),
                                                         QPaperStatus = dr["QPaperStatus"].ToString(),
                                                         UserName = dr["UserName"].ToString(),
                                                         PaperName = dr["QPaperName"].ToString(),
                                                         Marks = dr["Marks"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Marks"])
                                                     }).ToList();

                    model.StudentPaperDetailList = list;
                }
                #endregion

                return View("~/views/PaperAssignment/index.cshtml", model);
            }
            catch (Exception ex)
            {
                return View("~/views/PaperAssignment/index.cshtml", new StudentPaperDetail());
            }
        }

        [NonAction]
        public SelectList ToSelectList(DataTable table, string valueField, string textField)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(new SelectListItem()
                {
                    Text = row[textField].ToString(),
                    Value = row[valueField].ToString()
                });
            }
            return new SelectList(list, "Value", "Text");
        }

        [HttpPost]
        [CheckSessionOutAttribute]
        public ActionResult AssignPaperToStudent(long UserID, long QPaperID)
        {
            try
            {
                if (UserID <= 0 || QPaperID <= 0 )
                    throw new Exception("Invalid parameter!!!");

                var model = new StudentPaperDetail();
                DataTable dtResult = new DataTable();

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetStudentPaperDetail_GetByPaperAndUserIDs", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@QPaperID", QPaperID);
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtResult);
                        }
                    }
                }
                if (dtResult.Rows.Count > 0)
                    throw new Exception("Question paper is already assign to User, Please assign some other.");
                else
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_InsertNewStudentPaperDetail_Create", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QPaperID", QPaperID);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@QPaperStatus", "Assigned");
                            using (SqlDataAdapter da = new SqlDataAdapter())
                            {
                                da.SelectCommand = cmd;
                                da.Fill(dtResult);
                            }
                        }
                    }
                    return Json("The question paper was assigned successfully to the User.");
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}