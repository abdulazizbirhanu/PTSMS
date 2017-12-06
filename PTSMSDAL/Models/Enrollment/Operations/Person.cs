using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Scheduling.References;

namespace PTSMSDAL.Models.Enrollment.Operations
{
    [Table("PERSON")]
    public class Person : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PersonId { get; set; }

        [Index(IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Company Id Number is required.")]
        [Display(Name = "Company Id Number")]
        [MaxLength(32)]
        public string CompanyId { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Sex is required.")]
        [Display(Name = "Sex")]
        public string Sex { get; set; }

        [Required(ErrorMessage = "Birth Date is required.")]
        [Display(Name = "Birth Date")]
        public string BirthDate { get; set; }

        [Required(ErrorMessage = "Position is required.")]
        [Display(Name = "Position")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Registration Date is required.")]
        [Display(Name = "Registration Date")]
        public string RegistrationDate { get; set; }

        [ForeignKey("Location")]
        public int? LocationId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Nationality")]
        public string Nationality { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Photo URL")]
        [DataType(DataType.ImageUrl)]
        public string PhotoURL { get; set; }

        [Display(Name = "Contact Person 1 Name")]
        public string ContactPerson1Name { get; set; }

        [Display(Name = "Contact Person 1 Address")]
        public string ContactPerson1Address { get; set; }

        [Display(Name = "Contact Person 1 Phone")]
        public string ContactPerson1Phone { get; set; }

        [Display(Name = "Contact Person 2 Name")]
        public string ContactPerson2Name { get; set; }

        [Display(Name = "Contact Person 2 Address")]
        public string ContactPerson2Address { get; set; }

        [Display(Name = "Contact Person 2 Phone")]
        public string ContactPerson2Phone { get; set; }

        public virtual Location Location { get; set; }
    }
}