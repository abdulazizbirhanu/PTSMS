using PTSMSDAL.Access.Enrollment.Relations;
using PTSMSDAL.Models.Enrollment.Relations;

namespace PTSMSBAL.Logic.Enrollment.Relations
{
    public class TraineeBatchLogic
    {
        TraineeSyllabusAccess traineeBatchAccess = new TraineeSyllabusAccess();

        public object List()
        {
            return traineeBatchAccess.List();
        }

        public object Details(int id)
        {
            return traineeBatchAccess.Details(id);
        }

        /*
        public object Add(TraineeSyllabus traineeBatch)
        {
            return traineeBatchAccess.Add(traineeBatch);
        }

        public object Revise(TraineeSyllabus traineeBatch)
        {
            return traineeBatchAccess.Revise(traineeBatch);
        }
        */

        public object Delete(int id)
        {
            return traineeBatchAccess.Delete(id);
        }
    }
}