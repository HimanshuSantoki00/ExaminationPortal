using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExaminationPortal.Models
{
    public class QuestionBank
    {
        public long QBankID { get; set; }

        public long QBankDetailID { get; set; }

        public long QuestionID { get; set; }

        [Required]
        [DisplayName("Question Bank")]
        public string QBankName { get; set; }
    }

    public class QBankAndQuestionMapping
    {
        public long Value { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }
    }
    public class QBankAndQuestionMappingList
    {
        public List<QBankAndQuestionMapping> MappingList { get; set; }

        public QBankAndQuestionMappingList()
        {
            MappingList = new List<QBankAndQuestionMapping>();
        }
    }
}