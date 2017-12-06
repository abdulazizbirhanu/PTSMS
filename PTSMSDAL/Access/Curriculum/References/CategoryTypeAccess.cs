using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.References;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.References
{
    public class CategoryTypeAccess
    {
        private PTSContext db = new PTSContext();

        public List<CategoryType> List()
        {
            return db.CategoryTypes.Include(c => c.PreviousCategoryType).Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public List<CategoryType> CategoryTypeList()
        {
            return db.CategoryTypes.Include(c => c.PreviousCategoryType).Where(c => c.Status == "Active" && !c.Type.ToLower().Contains("ground") && c.EndDate > DateTime.Now).ToList();
        }

        public CategoryType Details(int id)
        {
            try
            {
                CategoryType categoryType = db.CategoryTypes.Find(id);
                if (categoryType == null)
                {
                    return null; // Not Found
                }
                return categoryType; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(CategoryType categoryType)
        {
            try
            {
                categoryType.StartDate = DateTime.Now;
                categoryType.EndDate = Constants.EndDate;
                categoryType.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                categoryType.CreationDate = DateTime.Now;

                db.CategoryTypes.Add(categoryType);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(CategoryType categoryType)
        {
            try
            {
                categoryType.RevisionDate = DateTime.Now;
                categoryType.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(categoryType).State = EntityState.Modified;
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
                CategoryType categoryType = db.CategoryTypes.Find(id);
                categoryType.EndDate = DateTime.Now;
                categoryType.Type += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(categoryType).State = EntityState.Modified;
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