using System.Collections.Generic;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.View;
using PTSMSDAL.Models.Enrollment.Relations;

namespace PTSMSBAL.Scheduling.Relations
{
    public class TraineeEvaluationTemplateLogic
    {
        public List<TraineeEvaluationTemplateView> EvaluationTemplateList(int traineeId, int lessonId, int sequence)
        {
            TraineeEvaluationTemplateAccess evaluationTemplateAccess = new TraineeEvaluationTemplateAccess();
            return evaluationTemplateAccess.EvaluationTemplateList(traineeId, lessonId, sequence);
        }
        public OperationResult SaveTraineeEvaluationTemplateItems(string[] evaluationTemplateItems, int overAllGradeId, int traineeId, int lessonId, string remark, int fylingFTDScheduleId, string TimeIn, string TimeOut, string FlightTime, string FlightDate)
        {
            TraineeEvaluationTemplateAccess evaluationTemplateAccess = new TraineeEvaluationTemplateAccess();
            OperationResult operationResult = evaluationTemplateAccess.SaveTraineeEvaluationTemplateItems(evaluationTemplateItems, overAllGradeId, traineeId, lessonId, remark, TimeIn, TimeOut, FlightTime, FlightDate);

            if (operationResult.IsSuccess)
            {
                FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();
                fTDAndFlyingSchedulerAccess.UpdateScheduleStatus(fylingFTDScheduleId, FlyingFTDScheduleStatus.Evaluated);
                return operationResult;
            }
            return operationResult;
        }
        public string AcceptEvaluationTemplate(int traineeId, int lessonId, bool isAccepted, int fylingFTDScheduleId)
        {
            TraineeEvaluationTemplateAccess evaluationTemplateAccess = new TraineeEvaluationTemplateAccess();
            OperationResult operationResult = evaluationTemplateAccess.AcceptEvaluationTemplate(traineeId, lessonId, isAccepted);

            if (operationResult.IsSuccess)
            {
                FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();
                FlyingFTDScheduleStatus status = new FlyingFTDScheduleStatus();
                if (isAccepted)
                    status = FlyingFTDScheduleStatus.Completed;
                else
                    status = FlyingFTDScheduleStatus.Unaccepted;
                fTDAndFlyingSchedulerAccess.UpdateScheduleStatus(fylingFTDScheduleId, status);
                return operationResult.Message;
            }
            return operationResult.Message;
        }

        public TraineeLesson CloneEvaluationTemplate(int traineeId, int lessonId)
        {
            TraineeEvaluationTemplateAccess evaluationTemplateAccess = new TraineeEvaluationTemplateAccess();
            return evaluationTemplateAccess.CloneEvaluationTemplate(traineeId, lessonId);
        }

        public bool IsTraineeLessonHasEvaluationItem(int traineeId, int lessonId, int sequence)
        {
            TraineeEvaluationTemplateAccess evaluationTemplateAccess = new TraineeEvaluationTemplateAccess();
            return evaluationTemplateAccess.IsTraineeLessonHasEvaluationItem(traineeId, lessonId, sequence);
        }
        public bool IsAlreadyEvaluated(int traineeId, int lessonId, int sequence)
        {
            TraineeEvaluationTemplateAccess evaluationTemplateAccess = new TraineeEvaluationTemplateAccess();
            return evaluationTemplateAccess.IsAlreadyEvaluated(traineeId, lessonId, sequence);
        }


        public bool PopulateEvaluationItemForTraineeLesson(int traineeId, int lessonId, int sequence)
        {
            TraineeEvaluationTemplateAccess evaluationTemplateAccess = new TraineeEvaluationTemplateAccess();
            return evaluationTemplateAccess.PopulateEvaluationItemForTraineeLesson(traineeId, lessonId, sequence);
        }
    }
}
