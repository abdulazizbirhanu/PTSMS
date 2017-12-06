using System;
using System.Data.Entity;
using System.Linq;
using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;

namespace PTSMSDAL.Access.Curriculum.Operations
{
    public class EvaluationItemAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.EvaluationItems.Where(c => c.EndDate >= DateTime.Now).OrderBy(c=>c.EvaluationCategoryId).ToList();
        }

        public object Details(int id)
        {
            try
            {
                EvaluationItem evaluationItem = db.EvaluationItems.Find(id);
                if (evaluationItem == null)
                {
                    return false; // Not Found
                }
                return evaluationItem; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Add(EvaluationItem evaluationItem)
        {
            try
            {
                evaluationItem.StartDate = DateTime.Now;
                evaluationItem.EndDate = Constants.EndDate;
                evaluationItem.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                evaluationItem.CreationDate = DateTime.Now;

                db.EvaluationItems.Add(evaluationItem);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(EvaluationItem evaluationItem)
        {
            evaluationItem.EndDate = Convert.ToDateTime("12/12/9999");
            evaluationItem.StartDate = DateTime.Now;
            evaluationItem.CreationDate = DateTime.Now;
            try
            {
                evaluationItem.RevisionDate = DateTime.Now;
                evaluationItem.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(evaluationItem).State = EntityState.Modified;
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
                EvaluationItem evaluationItem = db.EvaluationItems.Find(id);
                evaluationItem.EndDate = DateTime.Now;
                evaluationItem.EvaluationItemName += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(evaluationItem).State = EntityState.Modified;
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