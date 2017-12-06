using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSDAL.Models.Scheduling.View;
using System.Web;

namespace PTSMSDAL.Access.Scheduling.Relations
{
    public class EquipmentAccess
    {
        PTSContext db = new PTSContext();
        public List<Equipment> List()
        {           
            return db.Equipments/*.Where(E=>E.EquipmentStatus == "Active")*/.ToList();
        }

        public List<EquipmentView> get_EquipmentList()
        {
            //select Equipment it's certificate is not expired
            try
            {              
                List<EquipmentView> equipmentList = new List<EquipmentView>();
                DateTime today = DateTime.Now;

                var result = (from E in db.Equipments
                              join EC in db.EquipmentCertificates on E.EquipmentId equals EC.EquipmentId
                              where EC.EndingDate > today

                              select new EquipmentView
                              {
                                  EquipmentId = E.EquipmentId
                              }).ToList();

                var equipments = result.Distinct().ToList();
                foreach (var equipment in equipments)
                {
                    equipmentList.Add(equipment);
                }
                return equipmentList;
            }
            catch (Exception)
            {

                return new List<EquipmentView>();
            }

        }

        public Equipment Details(int id)
        {
            try
            {               
                Equipment equipment = db.Equipments.Find(id);
                if (equipment == null)
                {
                    return null; // Not Found
                }
                return equipment; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public object Add(Equipment equipment)
        {
            try
            {
                equipment.StartDate = DateTime.Now;
                equipment.EndDate = DateTime.MaxValue;
                equipment.CreatedBy = HttpContext.Current.User.Identity.Name;
                equipment.CreationDate = DateTime.Now;

                db.Equipments.Add(equipment);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(Equipment equipment)
        {
            try
            {
                equipment.RevisionDate = DateTime.Now;
                equipment.RevisedBy = HttpContext.Current.User.Identity.Name;
                db.Entry(equipment).State = EntityState.Modified;
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
                Equipment equipment = db.Equipments.Find(id);
                db.Equipments.Remove(equipment);
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
