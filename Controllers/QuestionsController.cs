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
    public class QuestionsController : Controller
    {
        // GET: Questions
        [CheckSessionOutAttribute]
        public ActionResult Index()
        {
            try
            {
                DataTable dtResult = new DataTable();

                #region Get Questions list
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetAllQuestionsList_Get", con))
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
                    List<Questions> questionList = (from dr in dtResult.AsEnumerable()
                                                    select new Questions()
                                                    {
                                                        QuestionID = Convert.ToInt64(dr["QuestionID"]),
                                                        Question = Convert.ToString(dr["Question"]),
                                                        OptionA = Convert.ToString(dr["OptionA"]),
                                                        OptionB = Convert.ToString(dr["OptionB"]),
                                                        OptionC = Convert.ToString(dr["OptionC"]),
                                                        OptionD = Convert.ToString(dr["OptionD"]),
                                                        CorrectAns = Convert.ToString(dr["CorrectAns"]),
                                                    }).ToList();

                    return View("~/views/Questions/index.cshtml", questionList);
                }
                else
                    return View("~/views/Questions/index.cshtml", new List<Questions>());
                #endregion
            }
            catch (Exception ex)
            {
                return View("~/views/Questions/index.cshtml", new List<Questions>());
            }
        }

        [HttpGet]
        [CheckSessionOutAttribute]
        public ActionResult Create()
        {
            return View(new Questions());
        }

        [HttpPost]
        [CheckSessionOutAttribute]
        public ActionResult Create(Questions model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("usp_InsertNewQuestion_Create", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Question", model.Question);
                            cmd.Parameters.AddWithValue("@OptionA", model.OptionA);
                            cmd.Parameters.AddWithValue("@OptionB", model.OptionB);
                            cmd.Parameters.AddWithValue("@OptionC", model.OptionC);
                            cmd.Parameters.AddWithValue("@OptionD", model.OptionD);
                            cmd.Parameters.AddWithValue("@CorrectAns", model.CorrectAns.ToUpper());
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    return RedirectToAction("Create");
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
            var model = new Questions();
            try
            {
                DataTable dtResult = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetQuestionsRecord_GetByID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@QuestionID", id);
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtResult);
                        }
                    }
                }
                
                if (dtResult.Rows.Count > 0)
                {
                    model.QuestionID = Convert.ToInt64(dtResult.Rows[0]["QuestionID"]);
                    model.Question = dtResult.Rows[0]["Question"].ToString();
                    model.OptionA = dtResult.Rows[0]["OptionA"].ToString();
                    model.OptionB = dtResult.Rows[0]["OptionB"].ToString();
                    model.OptionC = dtResult.Rows[0]["OptionC"].ToString();
                    model.OptionD = dtResult.Rows[0]["OptionD"].ToString();
                    model.CorrectAns = dtResult.Rows[0]["CorrectAns"].ToString();
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
        public ActionResult Edit(int id, Questions model)
        {
            try
            {
                if (id <= 0)
                    throw new  ArgumentException("QuestionID is invalid.");

                if (ModelState.IsValid)
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("usp_UpdateQuestion_Update", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QuestionID", id);
                            cmd.Parameters.AddWithValue("@Question", model.Question);
                            cmd.Parameters.AddWithValue("@OptionA", model.OptionA);
                            cmd.Parameters.AddWithValue("@OptionB", model.OptionB);
                            cmd.Parameters.AddWithValue("@OptionC", model.OptionC);
                            cmd.Parameters.AddWithValue("@OptionD", model.OptionD);
                            cmd.Parameters.AddWithValue("@CorrectAns", model.CorrectAns);
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