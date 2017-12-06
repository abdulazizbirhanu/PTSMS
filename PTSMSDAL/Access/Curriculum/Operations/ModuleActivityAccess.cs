using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Operations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Curriculum.Operations
{
    public class ModuleActivityAccess
    {
        private PTSContext db = new PTSContext();
        public List<ModuleActivity> List()
        {
            return db.ModuleActivitys.ToList();
        }

        public ModuleActivity Details(int id)
        {
            try
            {
                return db.ModuleActivitys.Find(id);
            }
            catch (Exception e)
            {
                return new ModuleActivity(); // Exception
            }
        }


        public bool Add(ModuleActivity moduleActivity)
        {
            try
            {
                db.ModuleActivitys.Add(moduleActivity);
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public bool Revise(ModuleActivity moduleActivity)
        {
            try
            {
                ModuleActivity moduleActivityObj = db.ModuleActivitys.Find(moduleActivity.ModuleActivityId);
                moduleActivityObj.ModuleActivityName = moduleActivity.ModuleActivityName;
                moduleActivityObj.EstimatedDuration = moduleActivity.EstimatedDuration;
                moduleActivityObj.ModuleId = moduleActivity.ModuleId;
                moduleActivityObj.RevisionDate = DateTime.Now;
                moduleActivityObj.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(moduleActivityObj).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                ModuleActivity moduleActivity = db.ModuleActivitys.Find(id);
                db.Entry(moduleActivity).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<ModuleActivity> GetModuleActivityList(int moduleId)
        {
            return db.ModuleActivitys.Where(MA => MA.ModuleId == moduleId).ToList();
        }
    }
}
