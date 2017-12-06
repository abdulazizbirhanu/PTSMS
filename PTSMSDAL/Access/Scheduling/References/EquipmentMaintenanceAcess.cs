using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;

namespace PTSMSDAL.Access.Scheduling.References
{
    public class EquipmentMaintenanceAcess
    {
        private PTSContext db = new PTSContext();

        public List<EquipmentMaintenance> List()
        {
            var equipmentMaintenances = db.EquipmentMaintenances.Include(e => e.Equipment).ToList();
            return equipmentMaintenances;
        }

        public EquipmentMaintenance Details(int? id)
        {
            return db.EquipmentMaintenances.Find(id);
        }

        public object Add(EquipmentMaintenance equipmentMaintenance)
        {
            try
            {
                equipmentMaintenance.StartDate = DateTime.Now;
                equipmentMaintenance.EndDate = DateTime.MaxValue;
                equipmentMaintenance.CreatedBy = HttpContext.Current.User.Identity.Name;
                equipmentMaintenance.CreationDate = DateTime.Now;

                db.EquipmentMaintenances.Add(equipmentMaintenance);
                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object Revise(EquipmentMaintenance equipmentMaintenance)
        {
            try
            {
                //var item = Details(equipmentMaintenance.EquipmentMaintenanceId);
                //equipmentMaintenance.ActualMaintenanceHour = item.ActualMaintenanceHour;
                //equipmentMaintenance.ActualCalanderStartDate = item.ActualCalanderStartDate;
                //equipmentMaintenance.ActualCalanderEndDate = item.ActualCalanderEndDate;

                equipmentMaintenance.RevisionDate = DateTime.Now;
                equipmentMaintenance.RevisedBy = HttpContext.Current.User.Identity.Name;

                EquipmentMaintenance equipmentMainten = new EquipmentMaintenance();
                equipmentMainten = equipmentMaintenance;

                //db = new PTSContext();
                db.Entry(equipmentMainten).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public object Delete(EquipmentMaintenance equipmentMaintenance)
        {
            try
            {
                db.EquipmentMaintenances.Remove(equipmentMaintenance);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public object Dispose()
        {
            try
            {
                db.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
