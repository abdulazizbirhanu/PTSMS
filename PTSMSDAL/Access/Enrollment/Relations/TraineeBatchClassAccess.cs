using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Enrollment.Relations
{
    public class TraineeBatchClassAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.TraineeBatchClasses.ToList();
        }

        public List<TraineeBatchClass> TraineeBatchClassList(int batchClassId)
        {
            return db.TraineeBatchClasses.Where(tbc=>tbc.BatchClassId == batchClassId).ToList();
        }
        public object Details(int id)
        {
            try
            {
                TraineeBatchClass traineeBatchClass = db.TraineeBatchClasses.Find(id);
                if (traineeBatchClass == null)
                {
                    return false; // Not Found
                }
                return traineeBatchClass; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public TraineeBatchClass BatchClassDetails(int traineeId)
        {
            try
            {
                TraineeBatchClass traineeBatchClass = db.TraineeBatchClasses.Where(bc => bc.TraineeId == traineeId).ToList().FirstOrDefault();
                if (traineeBatchClass == null)
                {
                    return new TraineeBatchClass(); // Not Found
                }
                return traineeBatchClass; // Success
            }
            catch (System.Exception e)
            {
                return new TraineeBatchClass(); // Exception
            }
        }
        public object Add(TraineeBatchClass traineeBatchClass)
        {
            try
            {
                db.TraineeBatchClasses.Add(traineeBatchClass);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(TraineeBatchClass traineeBatchClass)
        {
            try
            {
                db.Entry(traineeBatchClass).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Delete(int id)
        {
            try
            {
                TraineeBatchClass traineeBatchClass = db.TraineeBatchClasses.Find(id);
                db.TraineeBatchClasses.Remove(traineeBatchClass);
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
