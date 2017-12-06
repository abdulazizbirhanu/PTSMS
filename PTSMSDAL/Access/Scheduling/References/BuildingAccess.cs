using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.References;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Scheduling.References
{
    public class BuildingAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.Buildings.ToList();
        }

        public object Details(int id)
        {
            try
            {
                Building building = db.Buildings.Find(id);
                if (building == null)
                {
                    return false; // Not Found
                }
                return building; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Add(Building building)
        {
            try
            {
                db.Buildings.Add(building);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(Building building)
        {
            try
            {
                db.Entry(building).State = EntityState.Modified;
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
                Building building = db.Buildings.Find(id);
                db.Buildings.Remove(building);
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