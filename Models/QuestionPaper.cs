using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExaminationPortal.Models
{
    public class QuestionPaper
    {
        public long QPaperID { get; set; }
        [Required]
        [DisplayName("Question Paper")]
        public string QPaperName { get; set; }

    }

    public class QPaperQBankMapping
    {
        public long QPaperID { get; set; }
        public string QPaperName { get; set; }
        public long QBankID { get; set; }
        public string QBankName { get; set; }
        public string QBankIDs { get; set; }
        public List<QPaperQBankMapping> QPaperQBankMappingList { get; set; }

        public QPaperQBankMapping()
        {
            QPaperQBankMappingList = new List<QPaperQBankMapping>();
            QBankIDs = string.Empty;
        }
    }
}