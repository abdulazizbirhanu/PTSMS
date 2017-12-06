using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling.References
{
    public class EquipmentTypeAccess
    {
    

        public List<EquipmentType> List()
        {
            PTSContext db = new PTSContext();
            return db.EquipmentTypes.ToList();
        }

        public EquipmentType Details(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                EquipmentType equipmentType = db.EquipmentTypes.Find(id);
                if (equipmentType == null)
                {
                    return new EquipmentType(); // Not Found
                }
                return equipmentType; // Success
            }
            catch (System.Exception e)
            {
                return new EquipmentType(); // Exception
            }
        }

        public bool Add(EquipmentType equipmentType)
        {
            try
            {
                equipmentType.StartDate = DateTime.Now;
                equipmentType.EndDate = new DateTime(9999, 12, 31);
                equipmentType.CreationDate = DateTime.Now;
                equipmentType.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                equipmentType.RevisionDate = DateTime.Now;
                equipmentType.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;

                PTSContext db = new PTSContext();
                db.EquipmentTypes.Add(equipmentType);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(EquipmentType equipmentType)
        {
            try
            {
                PTSContext db = new PTSContext();
                equipmentType.RevisionDate = DateTime.Now;
                equipmentType.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(equipmentType).State = EntityState.Modified;
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
                PTSContext db = new PTSContext();
                EquipmentType equipmentType = db.EquipmentTypes.Find(id);
                db.EquipmentTypes.Remove(equipmentType);
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
