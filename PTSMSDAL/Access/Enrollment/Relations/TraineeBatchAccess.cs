using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Relations;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Enrollment.Relations
{
    public class TraineeSyllabusAccess
    {
        private PTSContext db = new PTSContext();

        public List<TraineeBatchClass> List()
        {
            return db.TraineeBatchClasses.ToList();
        }       
        public TraineeBatchClass Details(int id)
        {
            try
            {
                TraineeBatchClass traineeBatch = db.TraineeBatchClasses.Find(id);
                if (traineeBatch == null)
                {
                    return null; // Not Found
                }
                return traineeBatch; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(TraineeBatchClass traineeBatch)
        {
            try
            {
                db.TraineeBatchClasses.Add(traineeBatch);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(TraineeBatchClass traineeBatch)
        {
            try
            {
                db.Entry(traineeBatch).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Delete(int id)
        {
            try
            {
                TraineeBatchClass traineeBatch = db.TraineeBatchClasses.Find(id);
                db.TraineeBatchClasses.Remove(traineeBatch);
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