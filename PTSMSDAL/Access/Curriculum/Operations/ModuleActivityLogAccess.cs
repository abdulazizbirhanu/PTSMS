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
    public class ModuleActivityLogAccess
    {
        private PTSContext db = new PTSContext();
        public List<ModuleActivityLog> List()
        {
            return db.ModuleActivityLogs.ToList();
        }

        public ModuleActivityLog Details(int id)
        {
            try
            {
                return db.ModuleActivityLogs.Find(id);
            }
            catch (Exception e)
            {
                return null; // Exception
            }
        }


        public bool Add(ModuleActivityLog moduleActivityLog)
        {
            try
            {
                db.ModuleActivityLogs.Add(moduleActivityLog);
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        
        public bool Revise(ModuleActivityLog moduleActivityLog)
        {
            try
            {
                db.Entry(moduleActivityLog).State = EntityState.Modified;
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
                ModuleActivityLog moduleActivityLog = db.ModuleActivityLogs.Find(id);
                db.Entry(moduleActivityLog).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
