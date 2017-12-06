using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling.Relations
{
 public   class InstructorEquipmentModelAccess
    {
        public List<InstructorEquipmentModel> List()
        {
            PTSContext db = new PTSContext();
            return db.InstructorEquipmentModels.ToList();
        }

        public InstructorEquipmentModel Details(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                InstructorEquipmentModel instructorEquipmentModel = db.InstructorEquipmentModels.Find(id);
                if (instructorEquipmentModel == null)
                {
                    return null; // Not Found
                }
                return instructorEquipmentModel; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public object Add(InstructorEquipmentModel instructorEquipmentModel)
        {
            try
            {
                PTSContext db = new PTSContext();

                
                db.InstructorEquipmentModels.Add(instructorEquipmentModel);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(InstructorEquipmentModel instructorEquipmentModel)
        {
            try
            {
                PTSContext db = new PTSContext();
                db.Entry(instructorEquipmentModel).State = EntityState.Modified;
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
                InstructorEquipmentModel instructorEquipmentModel = db.InstructorEquipmentModels.Find(id);
                db.InstructorEquipmentModels.Remove(instructorEquipmentModel);
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
