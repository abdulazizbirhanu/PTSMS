using PTSMSDAL.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Others.School
{
    [Table("SCHOOL")]
    public class School: AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SchoolId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "School Name")]
        [Index("UK_SCHOOL", IsUnique = true, Order = 1)]
        public string SchoolName { get; set; }

        [Required]
        [Display(Name = "School Code")]
        public string SchoolCode { get; set; }

        [Required]
        [Display(Name = "Database Name")]
        public string DatabaseName { get; set; }

        [Required]
        [Display(Name = "Server")]
        public string Server { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
