using PTSMSDAL.Access.Dispatch.Master;
using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Dispatch.Master
{
   public class OverallGradeLogic
    {
        OverallGradeAccess overallGradeAccess = new OverallGradeAccess();

        public List<OverallGrade> List()
        {
            return overallGradeAccess.List();
        }

        public OverallGrade Details(int id)
        {
            return overallGradeAccess.Details(id);
        }

        public bool Add(OverallGrade overallGrade)
        {
            overallGrade.Status = "Active";
            overallGrade.RevisionNo = 1;
            return overallGradeAccess.Add(overallGrade);
        }

        public bool Revise(OverallGrade overallGrade)
        {
            OverallGrade d = (OverallGrade)overallGradeAccess.Details(overallGrade.OverallGradeId);
            return overallGradeAccess.Revise(d);
        }

        public bool Delete(int id)
        {
            return overallGradeAccess.Delete(id);
        }
    }
}

