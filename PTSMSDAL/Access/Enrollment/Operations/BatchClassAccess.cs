using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Relations;
using System;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Enrollment.Operations
{
    public class BatchClassAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.BatchClasses.Where(c => c.EndDate > DateTime.Now).ToList();
        }
        public object List(int batchId)
        {
            return db.BatchClasses.Where(c => c.BatchId == batchId && c.EndDate > DateTime.Now).ToList();
        }

        //public object GetBatchClass(int batchId)
        //{
        //    return db.BatchClasses.Where(c => c.BatchId == batchId && c.EndDate > DateTime.Now).ToList();
        //}
        public TraineeBatchClass GetTraineeBatchClass(int traineeId)
        {
            return db.TraineeBatchClasses.Single(tr=> tr.TraineeId == traineeId);
        }

        public bool ChangeTraineeBatchClass(int traineeBatchClassId, int batchClassId)
        {
            var tbc = db.TraineeBatchClasses.Find(traineeBatchClassId);

            tbc.BatchClassId = batchClassId;
            db.Entry(tbc).State = EntityState.Modified;

            return db.SaveChanges() > 0;
        }

        public object Details(int id)
        {
            try
            {
                BatchClass batchClass = db.BatchClasses.Find(id);
                if (batchClass == null)
                {
                    return false; // Not Found
                }
                return batchClass; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }   
        public object Add(BatchClass batchClass)
        {
            try
            {
                batchClass.StartDate = DateTime.Now;
                batchClass.EndDate = Constants.EndDate;
                batchClass.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                batchClass.CreationDate = DateTime.Now;

                db.BatchClasses.Add(batchClass);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(BatchClass batchClass)
        {
            try
            {
                batchClass.RevisionDate = DateTime.Now;
                batchClass.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(batchClass).State = EntityState.Modified;
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
                BatchClass batchClass = db.BatchClasses.Find(id);
                batchClass.EndDate = DateTime.Now;
                batchClass.BatchClassName += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(batchClass).State = EntityState.Modified;
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