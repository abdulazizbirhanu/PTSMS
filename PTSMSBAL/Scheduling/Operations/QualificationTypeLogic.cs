using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.References;

namespace PTSMSBAL.Scheduling.Operations
{
    public class QualificationTypeLogic
    {
        QualificationTypeAccess qualificationTypeAccess = new QualificationTypeAccess();

        public List<QualificationType> List()
        {
            return qualificationTypeAccess.List();
        }

        public QualificationType Details(int id)
        {
            return qualificationTypeAccess.Details(id);
        }

        public object Add(QualificationType period)
        {
            return qualificationTypeAccess.Add(period);
        }

        public object Revise(QualificationType period)
        {
            return qualificationTypeAccess.Revise(period);
        }

        public object Delete(int id)
        {
            return qualificationTypeAccess.Delete(id);
        }
    }
}
