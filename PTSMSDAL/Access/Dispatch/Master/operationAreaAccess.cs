using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Dispatch.Master
{
    public class operationAreaAccess
    {
        private PTSContext db = new PTSContext();

        public List<OperationArea> List()
        {
            return db.OperationAreas.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public OperationArea Details(int id)
        {
            try
            {
                OperationArea operationArea = db.OperationAreas.Find(id);
                if (operationArea == null)
                {
                    return null; // Not Found
                }
                return operationArea; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(OperationArea operationArea)
        {
            try
            {

                operationArea.StartDate = DateTime.Now;
                operationArea.EndDate = Constants.EndDate;
                operationArea.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                operationArea.CreationDate = DateTime.Now;

                db.OperationAreas.Add(operationArea);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(OperationArea operationArea)
        {
            try
            {
                operationArea.RevisionDate = DateTime.Now;
                operationArea.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(operationArea).State = EntityState.Modified;
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
                OperationArea operationArea = db.OperationAreas.Find(id);
                operationArea.EndDate = DateTime.Now;
                db.Entry(operationArea).State = EntityState.Modified;
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

