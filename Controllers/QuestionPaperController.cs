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
    public class QuestionPaperController : Controller
    {
        #region Question Paper
        // GET: QuestionPaper
        [CheckSessionOutAttribute]
        public ActionResult Index()
        {
            try
            {
                DataTable dtResult = new DataTable();

                #region Get Questions list
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetAllQuestionPaperList_Get", con))
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
                    List<QuestionPaper> questionBankList = (from dr in dtResult.AsEnumerable()
                                                            select new QuestionPaper()
                                                            {
                                                                QPaperID = Convert.ToInt64(dr["QPaperID"]),
                                                                QPaperName = dr["QPaperName"].ToString()
                                                            }).ToList();

                    return View("~/views/QuestionPaper/index.cshtml", questionBankList);
                }
                else
                    return View("~/views/QuestionPaper/index.cshtml", new List<QuestionPaper>());
                #endregion
            }
            catch (Exception ex)
            {
                return View("~/views/QuestionPaper/index.cshtml", new List<QuestionPaper>());
            }
        }

        [HttpGet]
        [CheckSessionOutAttribute]
        public ActionResult Create()
        {
            return View(new QuestionPaper());
        }

        [HttpPost]
        [CheckSessionOutAttribute]
        public ActionResult Create(QuestionPaper model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    #region Check Question Paper name Exists or Not
                    DataTable dt = new DataTable();
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_CheckQuestionPapernameExists_GetByName", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QPaperName", model.QPaperName);
                            using (SqlDataAdapter da = new SqlDataAdapter())
                            {
                                da.SelectCommand = cmd;
                                da.Fill(dt);
                            }
                        }
                    }
                    if (dt.Rows.Count > 0)
                        throw new Exception("Question Paper is already exists!!!");
                    #endregion

                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("usp_InsertNewQuestionPaper_Create", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QPaperName", model.QPaperName);
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
        public ActionResult Edit(long id)
        {
            var model = new QuestionPaper();
            try
            {
                DataTable dtResult = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetQuestionPaperRecord_GetByID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@QPaperID", id);
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtResult);
                        }
                    }
                }

                if (dtResult.Rows.Count > 0)
                {
                    model.QPaperID = Convert.ToInt64(dtResult.Rows[0]["QPaperID"]);
                    model.QPaperName = dtResult.Rows[0]["QPaperName"].ToString();
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
        public ActionResult Edit(long id, QuestionPaper model)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("QPaperID is invalid.");

                if (ModelState.IsValid)
                {
                    #region Check Question Bank name Exists or Not while updating
                    DataTable dt = new DataTable();
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_CheckQuestionPapernameExistsWhileUpdate_GetByName", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QPaperID", id);
                            cmd.Parameters.AddWithValue("@QPaperName", model.QPaperName);
                            using (SqlDataAdapter da = new SqlDataAdapter())
                            {
                                da.SelectCommand = cmd;
                                da.Fill(dt);
                            }
                        }
                    }
                    if (dt.Rows.Count > 0)
                        throw new Exception("Question Paper is already exists!!!");
                    #endregion

                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("usp_UpdateQuestionPaper_Update", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QPaperID", id);
                            cmd.Parameters.AddWithValue("@QPaperName", model.QPaperName);
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

        [HttpGet]
        [CheckSessionOutAttribute]
        public ActionResult QPaperQBankMapping(long id, string name)
        {
            try
            {
                var model = new QPaperQBankMapping();
                DataTable dtQBankList = new DataTable();
                DataTable dtResult = new DataTable();

                #region Prepare dropdown data
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetQuestionBankListHaving4Ques_Get", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtQBankList);
                        }
                    }
                }
                ViewBag.QBankList = ToSelectList(dtQBankList, "QBankID", "QBankName");
                #endregion

                #region Get prepare list
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetPaperAndBankDetails_GetByPaperID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@QPaperID", id);
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dtResult);
                        }
                    }
                }
                #endregion

                #region Prepare data to rerurn into view
                ViewBag.QPaperID = id;
                ViewBag.QPaperName = name;
                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    List<QPaperQBankMapping> questionBankList = (from dr in dtResult.AsEnumerable()
                                                                 select new QPaperQBankMapping()
                                                                 {
                                                                     QPaperName = dr["QPaperName"].ToString(),
                                                                     QBankName = dr["QBankName"].ToString()
                                                                 }).ToList();

                    model.QPaperQBankMappingList = questionBankList;
                    ViewBag.IsAllQBankMapped = 1;
                }
                else
                {
                    ViewBag.IsAllQBankMapped = 0;
                }
                return View("~/views/QuestionPaper/QPaperQBankMapping.cshtml", model);
                #endregion
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [CheckSessionOutAttribute]
        public ActionResult QPaperQBankMappingSave(long QPaperID, string selectQBankList)
        {
            try
            {
                #region Prepare data
                List<List<long>> list = new List<List<long>>();
                string[] selectQBankListArray = selectQBankList.Split(',');
                foreach (string val in selectQBankListArray)
                {
                    DataTable dtQBankDetail = new DataTable();
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_GetQuestionBankData_GetByQBankIDs", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QBankIDs", Convert.ToInt64(val));
                            using (SqlDataAdapter da = new SqlDataAdapter())
                            {
                                da.SelectCommand = cmd;
                                da.Fill(dtQBankDetail);
                            }
                        }
                    }

                    List<long> tempList = dtQBankDetail.AsEnumerable().Select(x => Convert.ToInt64(x["QBankDetailID"])).ToList();
                    if (tempList.Count < 4)
                        throw new ArgumentException("Something wend wrong!!!");

                    list.Add(tempList);
                }

                if (list.Count < 4)
                    throw new ArgumentException("Something wend wrong!!!");
                #endregion

                #region Random ID selection
                var random = new Random();
                List<long> randomQBankDetailIDList = new List<long>();
                foreach (List<long> item in list)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int index = random.Next(item.Count);
                        randomQBankDetailIDList.Add(item[index]);
                        item.RemoveAt(index);
                    }
                }

                if (randomQBankDetailIDList.Count != 16)
                    throw new ArgumentException("Something wend wrong!!!");
                #endregion

                #region Prepare datatable for bulk Insert
                DataTable dt = new DataTable();
                dt.Columns.Add("QPaperID", typeof(long));
                dt.Columns.Add("QBankDetailID", typeof(long));
                foreach (long val in randomQBankDetailIDList)
                {
                    dt.Rows.Add(QPaperID, Convert.ToInt64(val));
                }
                #endregion

                #region Bulk Insert
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnection"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("usp_InsertQPaperQBankDetailsBulk_Create", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@tblType", dt);
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                return Content("true");
                #endregion
            }
            catch (Exception ex)
            {
                return Content("false");
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
    }
}