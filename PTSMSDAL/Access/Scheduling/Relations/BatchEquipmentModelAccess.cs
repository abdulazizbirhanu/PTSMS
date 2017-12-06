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
   public class BatchEquipmentModelAccess
    {
        public List<BatchEquipmentModel> List()
        {
            PTSContext db = new PTSContext();
            return db.BatchEquipmentModels.ToList();
        }

        public BatchEquipmentModel Details(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                BatchEquipmentModel batchEquipmentModel = db.BatchEquipmentModels.Find(id);
                if (batchEquipmentModel == null)
                {
                    return null; // Not Found
                }
                return batchEquipmentModel; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public object Add(BatchEquipmentModel batchEquipmentModel)
        {
            try
            {
                PTSContext db = new PTSContext();

                batchEquipmentModel.StartDate = DateTime.Now;
                batchEquipmentModel.EndDate = new DateTime(9999, 12, 31);
                batchEquipmentModel.CreationDate = DateTime.Now;
                batchEquipmentModel.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                batchEquipmentModel.RevisionDate = DateTime.Now;
                batchEquipmentModel.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;

                db.BatchEquipmentModels.Add(batchEquipmentModel);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(BatchEquipmentModel batchEquipmentModel)
        {
            try
            {
                PTSContext db = new PTSContext();
                db.Entry(batchEquipmentModel).State = EntityState.Modified;
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
                BatchEquipmentModel batchEquipmentModel = db.BatchEquipmentModels.Find(id);
                db.BatchEquipmentModels.Remove(batchEquipmentModel);
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
