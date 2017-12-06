using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Others.View
{
    public class InstructorProfileView
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Company Id")]
        public string CompanyId { get; set; }

        [Display(Name = "Sex")]
        public string Sex { get; set; }

        [Display(Name = "Birth Date")]
        public string BirthDate { get; set; }

        [Display(Name = "Position")]
        public string Position { get; set; }

        [Display(Name = "Registration Date")]
        public string RegistrationDate { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Nationality")]
        public string Nationality { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }        

        [Display(Name = "Qualification")]
        public string Qualification { get; set; }
    }
}
