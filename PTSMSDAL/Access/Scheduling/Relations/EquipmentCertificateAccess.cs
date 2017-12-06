using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Relations;

namespace PTSMSDAL.Access.Scheduling.Relations
{
    public class EquipmentCertificateAccess
    {
        public List<EquipmentCertificate> List()
        {
            PTSContext db = new PTSContext();
            return db.EquipmentCertificates.ToList();
        }

        public EquipmentCertificate Details(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                EquipmentCertificate equipmentCertificate = db.EquipmentCertificates.Find(id); 
                if (equipmentCertificate == null)
                {
                    return null; // Not Found
                }
                return equipmentCertificate; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public object Add(EquipmentCertificate equipmentCertificate)
        {
            try
            {
                PTSContext db = new PTSContext();
                db.EquipmentCertificates.Add(equipmentCertificate);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(EquipmentCertificate equipmentCertificate)
        {
            try
            {
                PTSContext db = new PTSContext();
                db.Entry(equipmentCertificate).State = EntityState.Modified;
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
                EquipmentCertificate equipmentCertificate = db.EquipmentCertificates.Find(id);
                db.EquipmentCertificates.Remove(equipmentCertificate);
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
