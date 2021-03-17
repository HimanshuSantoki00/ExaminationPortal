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
    public class StudentExamController : Controller
    {
        // GET: StudentExam
        [CheckSessionOutAttribute]
        public ActionResult Index(long StudentPaperDetailID)
        {
            var model = new StudentPaperDetailList();
            try
            {
                ViewBag.StudentPaperDetailID = StudentPaperDetailID;
                DataTable dtResult = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetStudentPaperDetail", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StudentPaperDetailID", StudentPaperDetailID);
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtResult);
                        }
                    }
                }

                if (dtResult.Rows.Count > 0)
                {
                    foreach (DataRow item in dtResult.Rows)
                    {
                        model.MappingList.Add(new StudentPaperDetail
                        {
                            StudentPaperDetailID = Convert.ToInt64(item["StudentPaperDetailID"]),
                            Time = Convert.ToString(item["Time"]) == "" ? 0 : Convert.ToInt64(item["Time"]),
                            QPaperName = Convert.ToString(item["QPaperName"]),
                            QuestionID = Convert.ToInt64(item["QuestionID"]),
                            Question = Convert.ToString(item["Question"]),
                            OptionA = Convert.ToString(item["OptionA"]),
                            OptionB = Convert.ToString(item["OptionB"]),
                            OptionC = Convert.ToString(item["OptionC"]),
                            OptionD = Convert.ToString(item["OptionD"]),
                            SubmittedAns = Convert.ToString(item["SubmittedAns"])
                        });
                    }
                    ViewBag.PaperName = dtResult.Rows[0]["QPaperName"];
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

        [CheckSessionOutAttribute]
        [HttpPost]
        public ActionResult SaveStudentPaperDetailAnswer(long StudentPaperDetailID, string questionids, string answers)
        {
            string[] que = questionids.Split(',');
            string[] ans = answers.Split(',');

            for (int i = 0; i < que.Count(); i++)
            {
                if (!String.IsNullOrEmpty(que[i]))
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("usp_InsertStudentPaperDetailAnswer_Create", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@StudentPaperDetailID", StudentPaperDetailID);
                            cmd.Parameters.AddWithValue("@QuestionID", que[i]);
                            cmd.Parameters.AddWithValue("@SubmittedAns", ans[i]);
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
            }

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("usp_UpdateStudentPaperDetail_Update", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StudentPaperDetailID", StudentPaperDetailID);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }

            return RedirectToAction("Index");
        }

        [CheckSessionOutAttribute]
        public ActionResult AssignedPaper()
        {
            try
            {
                DataTable dtResult = new DataTable();

                #region Get assigned paper list
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetAllStudentPaperDetailList_GetByUserID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", Convert.ToInt64(Session["UserID"]));
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
                                                         Marks = dr["Marks"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Marks"]),
                                                         IsExamCompleted = dr["QPaperStatus"].ToString().Trim() == "Assigned" ? false : true
                                                     }).ToList();

                    return View("~/views/StudentExam/AssignedPaper.cshtml", list);
                }
                else
                    return View("~/views/StudentExam/AssignedPaper.cshtml", new List<StudentPaperDetail>());
                #endregion
            }
            catch (Exception ex)
            {
                return View("~/views/StudentExam/AssignedPaper.cshtml", new List<StudentPaperDetail>());
            }
        }
    }
}