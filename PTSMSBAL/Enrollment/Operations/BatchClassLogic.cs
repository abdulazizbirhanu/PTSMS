using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Relations;

namespace PTSMSBAL.Logic.Enrollment.Operations
{
    public class BatchClassLogic
    {
        BatchClassAccess batchClassAccess = new BatchClassAccess();

        public object List()
        {
            return batchClassAccess.List();
        }
        public object List(int batchId)
        {
            return batchClassAccess.List(batchId);
        }
        //public object GetBatchClass(int batchClassId)
        //{
        //    return batchClassClassAccess.GetBatchClass(batchClassId);
        //}
        public TraineeBatchClass GetTraineeBatchClass(int traineeId)
        {
            return batchClassAccess.GetTraineeBatchClass(traineeId);
        }
        public bool ChangeTraineeBatchClass(int traineeBatchClassId, int batchClassId)
        {
            return batchClassAccess.ChangeTraineeBatchClass(traineeBatchClassId, batchClassId);
        }

        public object Details(int id)
        {
            return batchClassAccess.Details(id);
        }
       

        public object Add(BatchClass batchClass)
        {
            return batchClassAccess.Add(batchClass);
        }

        public object Revise(BatchClass batchClass)
        {
            BatchClass bc = (BatchClass)batchClassAccess.Details(batchClass.BatchClassId);
            
            bc.BatchClassName = batchClass.BatchClassName;

            return batchClassAccess.Revise(bc);
        }

        public object Delete(int id)
        {
            return batchClassAccess.Delete(id);
        }
    }
}