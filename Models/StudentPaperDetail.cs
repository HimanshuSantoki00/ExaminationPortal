using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ExaminationPortal.Models
{
    public class StudentPaperDetail
    {
        public long StudentPaperDetailID { get; set; }
        public long UserID { get; set; }

        [DisplayName("User")]
        public string UserName { get; set; }
        public long QPaperID { get; set; }

        [DisplayName("Question Paper")]
        public string PaperName { get; set; }

        [DisplayName("Status")]
        public string QPaperStatus { get; set; }

        public bool IsExamCompleted { get; set; }
        public int? Marks { get; set; }
        public long? Time { get; set; }
        public List<SelectListItem> UserIDs { get; set; }
        public List<SelectListItem> QPaperIDs { get; set; }
        public List<StudentPaperDetail> StudentPaperDetailList { get; set; }

        public string QPaperName { get; set; }
        public long QuestionID { get; set; }
        public string Question { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string SubmittedAns { get; set; }

        public StudentPaperDetail()
        {
            Marks = 0;
            Time = 0;
            UserIDs = new List<SelectListItem>();
            QPaperIDs = new List<SelectListItem>();
            StudentPaperDetailList = new List<StudentPaperDetail>();
        }
    }

    public class StudentPaperDetailList
    {
        public List<StudentPaperDetail> MappingList { get; set; }

        public StudentPaperDetailList()
        {
            MappingList = new List<StudentPaperDetail>();
        }
    }
}