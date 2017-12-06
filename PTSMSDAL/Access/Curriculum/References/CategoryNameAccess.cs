using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.References;
using System;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.References
{
    public class CategoryNameAccess
    {
    //    private PTSContext db = new PTSContext();

    //    public object List()
    //    {
    //        return db.CategoryNames.Include(c => c.PreviousCategoryName).Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
    //    }

    //    public object Details(int id)
    //    {
    //        try
    //        {
    //            CategoryName categoryName = db.CategoryNames.Find(id);
    //            if (categoryName == null)
    //            {
    //                return false; // Not Found
    //            }
    //            return categoryName; // Success
    //        }
    //        catch (System.Exception e)
    //        {
    //            return false; // Exception
    //        }
    //    }

    //    public object Add(CategoryName categoryName)
    //    {
    //        try
    //        {
    //            categoryName.StartDate = DateTime.Now;
    //            categoryName.EndDate = Constants.EndDate;
    //            categoryName.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
    //            categoryName.CreationDate = DateTime.Now;

    //            db.CategoryNames.Add(categoryName);
    //            db.SaveChanges();
    //            return true; // Success
    //        }
    //        catch (System.Exception e)
    //        {
    //            return false; // Exception
    //        }
    //    }

    //    public object Revise(CategoryName categoryName)
    //    {
    //        try
    //        {
    //            categoryName.RevisionDate = DateTime.Now;
    //            categoryName.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;

    //            db.Entry(categoryName).State = EntityState.Modified;
    //            db.SaveChanges();
    //            return true;// Success
    //        }
    //        catch (System.Exception e)
    //        {
    //            return false; // Exception
    //        }
    //    }

    //    public object Delete(int id)
    //    {
    //        try
    //        {
    //            CategoryName categoryName = db.CategoryNames.Find(id);
    //            categoryName.EndDate = DateTime.Now;
    //            db.Entry(categoryName).State = EntityState.Modified;
    //            db.SaveChanges();
    //            return true;// Success
    //        }
    //        catch (System.Exception e)
    //        {
    //            return false; // Exception
    //        }
    //    }
    }
}