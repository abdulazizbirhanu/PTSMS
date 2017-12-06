using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Operations;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class ToolAccess
    {
        public List<Tool> List()
        {
            PTSContext db = new PTSContext();
            return db.Tools.ToList();
        }

        public Tool Details(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                Tool tool = db.Tools.Find(id);
                if (tool == null)
                {
                    return null; // Not Found
                }
                return tool; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public object Add(Tool tool)
        {
            try
            {
                PTSContext db = new PTSContext();
                db.Tools.Add(tool);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(Tool tool)
        {
            try
            {
                PTSContext db = new PTSContext();
                db.Entry(tool).State = EntityState.Modified;
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
                PTSContext db = new PTSContext();
                Tool tool = db.Tools.Find(id);
                db.Tools.Remove(tool);
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