using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.References;
using System;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.References
{
    public class StageAccess
    {
        private PTSContext db = new PTSContext();
        
        public object List()
        {
            return db.Stages.Where(p => p.EndDate > DateTime.Now).ToList();
        }

        public object Details(int id)
        {
            try
            {
                Stage stage = db.Stages.Find(id);
                if (stage == null)
                {
                    return null; // Not Found
                }
                return stage; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public object Add(Stage stage)
        {
            try
            {
                stage.StartDate = DateTime.Now;
                stage.EndDate = Constants.EndDate;
                stage.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                stage.CreationDate = DateTime.Now;

                db.Stages.Add(stage);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(Stage stage)
        {
            try
            {
                stage.RevisionDate = DateTime.Now;
                stage.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(stage).State = EntityState.Modified;
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
                Stage stage = db.Stages.Find(id);
                stage.EndDate = DateTime.Now;
                stage.Name += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(stage).State = EntityState.Modified;
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