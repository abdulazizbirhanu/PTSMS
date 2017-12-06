using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling.Relations
{
    public class BriefingAndDebriefingAccess
    {
        PTSContext db = new PTSContext();
        public List<BriefingAndDebriefing> List()
        {
            return db.BriefingAndDebriefings/*.Where(E=>E.EquipmentStatus == "Active")*/.ToList();
        }

        public BriefingAndDebriefing Details(int id)
        {
            try
            {
                BriefingAndDebriefing briefingAndDebriefing = db.BriefingAndDebriefings.Find(id);
                if (briefingAndDebriefing == null)
                {
                    return null; // Not Found
                }
                return briefingAndDebriefing; // Success
            }
            catch (Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(BriefingAndDebriefing briefingAndDebriefing)
        {
            try
            {
                db.BriefingAndDebriefings.Add(briefingAndDebriefing);
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(BriefingAndDebriefing briefingAndDebriefing)
        {
            try
            {
                db.Entry(briefingAndDebriefing).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false; 
            }
        }

        public bool Delete(int id)
        {
            try
            {
                BriefingAndDebriefing briefingAndDebriefing = db.BriefingAndDebriefings.Find(id);
                db.BriefingAndDebriefings.Remove(briefingAndDebriefing);
                return db.SaveChanges() > 0;               
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
