using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Operations;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Enrollment.Operations
{
    public class TraineeAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.Trainees.ToList();
        }
        public Trainee Details(int id)
        {
            try
            {
                Trainee trainee = db.Trainees.Find(id);
                if (trainee == null)
                {
                    return null; // Not Found
                }
                return trainee; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public object Add(Trainee trainee)
        {
            try
            {
                db.Trainees.Add(trainee);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(Trainee trainee)
        {
            try
            {
                db.Entry(trainee).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool UpdateCallSign(int personId, string callSign)
        {
            var person = db.Persons.Find(personId);
            person.ShortName = callSign;

            db.Entry(person).State = EntityState.Modified;
            return db.SaveChanges() > 0;
        }

        public object Delete(int id)
        {
            try
            {
                Trainee trainee = db.Trainees.Find(id);
                db.Trainees.Remove(trainee);
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
    }
}