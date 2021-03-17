using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExaminationPortal.Models
{
    public class User
    {
        public long UserID { get; set; }

        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required]
        [DisplayName("User Password")]
        public string UserPassword { get; set; }

        public int UserType { get; set; }

        [DisplayName("Is Teacher")]
        public bool IsTeacher { get; set; }
    }
}