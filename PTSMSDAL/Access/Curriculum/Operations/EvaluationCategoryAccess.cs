using System;
using System.Data.Entity;
using System.Linq;
using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;

namespace PTSMSDAL.Access.Curriculum.Operations
{
    public class EvaluationCategoryAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.EvaluationCategories.Where(c => c.EndDate > DateTime.Now).ToList();
        }

        public object Details(int id)
        {
            try
            {
                EvaluationCategory evaluationCategory = db.EvaluationCategories.Find(id);
                if (evaluationCategory == null)
                {
                    return false; // Not Found
                }
                return evaluationCategory; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }


        public object Add(EvaluationCategory evaluationCategory)
        {
            try
            {
                evaluationCategory.StartDate = DateTime.Now;
                evaluationCategory.EndDate = Constants.EndDate;
                evaluationCategory.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                evaluationCategory.CreationDate = DateTime.Now;

                db.EvaluationCategories.Add(evaluationCategory);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(EvaluationCategory evaluationCategory)
        {
            try
            {
                evaluationCategory.EndDate = Convert.ToDateTime("12/12/9999");
                evaluationCategory.StartDate = DateTime.Now;
                evaluationCategory.RevisionDate = DateTime.Now;
                evaluationCategory.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(evaluationCategory).State = EntityState.Modified;
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
                EvaluationCategory evaluationCategory = db.EvaluationCategories.Find(id);
                evaluationCategory.EndDate = DateTime.Now;
                evaluationCategory.EvaluationCategoryName += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(evaluationCategory).State = EntityState.Modified;
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