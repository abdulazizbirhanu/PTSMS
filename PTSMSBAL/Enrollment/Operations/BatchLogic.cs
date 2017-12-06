using System.Collections.Generic;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Others.View;

namespace PTSMSBAL.Logic.Enrollment.Operations
{
    public class BatchLogic
    {
        BatchAccess batchAccess = new BatchAccess();

        public List<resultSet> EnrollTrainee(int batchId, int traineeId, string CompanyId, ref List<resultSet> resultSet)
        {
            return batchAccess.EnrollBatch(batchId, traineeId, CompanyId, ref resultSet);
        }

        public object ListTrainee(int batchId)
        {
            return batchAccess.ListTrainee(batchId);
        }

        public object AddTraineeToBatchClass(TraineeBatchClass traineeBatchClass)
        {
            return batchAccess.AddTraineeToBatchClass(traineeBatchClass);
        }
        /*nice*/
        public object AddSyllabus(int batchId)
        {
            return batchAccess.GenerateSyllabus(batchId);
        }
        public object ListTraineeToBatchClass(int batchClassId)
        {
            return batchAccess.ListTraineeToBatchClass(batchClassId);
        }

        public object DeleteBatchTrainee(int id)
        {
            return batchAccess.DeleteBatchTrainee(id);
        }
        public object List()
        {
            return batchAccess.List();
        }

        public object Details(int id)
        {
            return batchAccess.Details(id);
        }

        public object Add(Batch batch)
        {
            return batchAccess.Add(batch);
        }

        public object Revise(Batch batch)
        {
            Batch b = (Batch)batchAccess.Details(batch.BatchId);
            b.ProgramId = batch.ProgramId;
            b.BatchName = batch.BatchName;
            b.BatchStartDate = batch.BatchStartDate;
            b.EstimatedEndDate = batch.EstimatedEndDate;

            return batchAccess.Revise(b);
        }

        public object Delete(int id)
        {
            return batchAccess.Delete(id);
        }

        public object GetTraineeHierarchy(int traineeId)
        {
            return batchAccess.GetTraineeHierarchy(traineeId);
        }
        /*
        public List<CurriculumChangeView> ShowCurriculumChange(int batchId)
        {
            return batchAccess.ShowCurriculumChange(batchId);
        }
        */
    }
}