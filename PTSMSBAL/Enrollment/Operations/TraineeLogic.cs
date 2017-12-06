using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Operations;

namespace PTSMSBAL.Logic.Enrollment.Operations
{
    public class TraineeLogic
    {
        TraineeAccess traineeAccess = new TraineeAccess();

        public object List()
        {
            return traineeAccess.List();
        }

        public object Details(int id)
        {
            return traineeAccess.Details(id);
        }
        public Trainee DetailsTrainee(int id)
        {
            return traineeAccess.Details(id);
        }

        public object Add(Trainee trainee)
        {
            return traineeAccess.Add(trainee);
        }

        public object Revise(Trainee trainee)
        {
            return traineeAccess.Revise(trainee);
        }

        public bool UpdateCallSign(int personId, string callSign)
        {
            return traineeAccess.UpdateCallSign(personId, callSign);
        }

        public object Delete(int id)
        {
            return traineeAccess.Delete(id);
        }
    }
}