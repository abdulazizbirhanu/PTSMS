using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Generic
{
    public class AuditAttribute
    {
        [ScaffoldColumn(false)]
        public DateTime StartDate { get; set; }

        [ScaffoldColumn(false)]
        public DateTime EndDate { get; set; }

        [ScaffoldColumn(false)]
        [Column(TypeName = "DateTime2")]
        public DateTime CreationDate { get; set; }

        [ScaffoldColumn(false)]
        public string CreatedBy { get; set; }

        [ScaffoldColumn(false)]
        [Column(TypeName = "DateTime2")]
        public DateTime RevisionDate { get; set; }

        [ScaffoldColumn(false)]
        public string RevisedBy { get; set; }
    }
}