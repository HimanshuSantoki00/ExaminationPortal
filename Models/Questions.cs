using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExaminationPortal.Models
{
    public class Questions
    {
        public long QuestionID { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        [DisplayName("Option A")]
        public string OptionA { get; set; }

        [Required]
        [DisplayName("Option B")]
        public string OptionB { get; set; }

        [Required]
        [DisplayName("Option C")]
        public string OptionC { get; set; }

        [Required]
        [DisplayName("Option D")]
        public string OptionD { get; set; }

        [Required]
        [DisplayName("Answer")]
        [RegularExpression(@"^[a-dA-D]+$", ErrorMessage = "Use letters from A to D only.")]
        public string CorrectAns { get; set; }

    }
}