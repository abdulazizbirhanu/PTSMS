﻿using PTSMSDAL.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Operations
{
    [Table("TRAINEE")]
    public class Trainee : AuditAttribute
    {
        [Key]
        [ForeignKey("Person")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeId { get; set; }        
        
        public virtual Person Person { get; set; }
    }
}