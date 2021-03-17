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
    public class QuestionBankController : Controller
    {
        #region Question Bank
        [CheckSessionOutAttribute]
        public ActionResult Index()
        {
            try
            {
                DataTable dtResult = new DataTable();

                #region Get Questions list
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetAllQuestionBankList_Get", con))
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
                    List<QuestionBank> questionBankList = (from dr in dtResult.AsEnumerable()
                                                           select new QuestionBank()
                                                           {
                                                               QBankID = Convert.ToInt64(dr["QBankID"]),
                                                               QBankName = dr["QBankName"].ToString()
                                                           }).ToList();

                    return View("~/views/QuestionBank/index.cshtml", questionBankList);
                }
                else
                    return View("~/views/QuestionBank/index.cshtml", new List<QuestionBank>());
                #endregion
            }
            catch (Exception ex)
            {
                return View("~/views/QuestionBank/index.cshtml", new List<QuestionBank>());
            }
        }

        [HttpGet]
        [CheckSessionOutAttribute]
        public ActionResult Create()
        {
            return View(new QuestionBank());
        }

        [HttpPost]
        [CheckSessionOutAttribute]
        public ActionResult Create(QuestionBank model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    #region Check Question Bank name Exists or Not
                    DataTable dt = new DataTable();
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_CheckQuestionBanknameExists_GetByName", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QBankName", model.QBankName);
                            using (SqlDataAdapter da = new SqlDataAdapter())
                            {
                                da.SelectCommand = cmd;
                                da.Fill(dt);
                            }
                        }
                    }
                    if (dt.Rows.Count > 0)
                        throw new Exception("Question Bank is already exists!!!");
                    #endregion

                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("usp_InsertNewQuestionBank_Create", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QBankName", model.QBankName);
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
            var model = new QuestionBank();
            try
            {
                DataTable dtResult = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetQuestionBankRecord_GetByID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@QBankID", id);
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtResult);
                        }
                    }
                }

                if (dtResult.Rows.Count > 0)
                {
                    model.QBankID = Convert.ToInt64(dtResult.Rows[0]["QBankID"]);
                    model.QBankName = dtResult.Rows[0]["QBankName"].ToString();
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
        public ActionResult Edit(int id, QuestionBank model)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("QBankID is invalid.");

                if (ModelState.IsValid)
                {
                    #region Check Question Bank name Exists or Not while updating
                    DataTable dt = new DataTable();
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_CheckQuestionBanknameExistsWhileUpdate_GetByName", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QBankID", id);
                            cmd.Parameters.AddWithValue("@QBankName", model.QBankName);
                            using (SqlDataAdapter da = new SqlDataAdapter())
                            {
                                da.SelectCommand = cmd;
                                da.Fill(dt);
                            }
                        }
                    }
                    if (dt.Rows.Count > 0)
                        throw new Exception("Question Bank is already exists!!!");
                    #endregion

                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("usp_UpdateQuestionBank_Update", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QBankID", id);
                            cmd.Parameters.AddWithValue("@QBankName", model.QBankName);
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
        #endregion

        [CheckSessionOutAttribute]
        public ActionResult AddQBankAndQuestionMapping(long id)
        {
            var model = new QBankAndQuestionMappingList();
            try
            {
                DataTable dtResult = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetAllUnMappedQuestion_Get", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtResult);
                        }
                    }
                }
                ViewBag.QBankID = id;

                if (dtResult.Rows.Count > 0)
                {
                    foreach (DataRow item in dtResult.Rows)
                    {
                        model.MappingList.Add(new QBankAndQuestionMapping
                        {
                            Value = Convert.ToInt64(item["QuestionID"]),
                            Text = item["Question"].ToString(),
                            IsChecked = false
                        });
                    }
                    return View(model);
                }
                else
                    return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [CheckSessionOutAttribute]
        public ActionResult AddQBankAndQuestionMapping(long QBankID, string selectedQuestionsID)
        {
            try
            {
                #region Prepare datatable for bulk Insert
                DataTable dt = new DataTable();
                dt.Columns.Add("QBankID", typeof(long));
                dt.Columns.Add("QuestionID", typeof(long));
                string[] values = selectedQuestionsID.Split(',');
                foreach (string val in values)
                {
                    dt.Rows.Add(QBankID, Convert.ToInt64(val));
                }
                #endregion

                #region Bulk Insert
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("usp_InsertQBankDetailsBulk_Create", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@tblType", dt);
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                return RedirectToAction("Index");
                #endregion
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("Index");
            }
        }
    }
}