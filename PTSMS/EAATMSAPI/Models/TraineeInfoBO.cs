﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAATMSAPI.Models
{
    [Table("TraineeInfo")]
    public class TraineeInfoBO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string CellPhone { get; set; }
        public string HomePhone { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string EducationalLevel { get; set; }
        public string ApplyingForProgram { get; set; }
        public string CertificateType { get; set; }
        public string Category { get; set; }

    }
}
