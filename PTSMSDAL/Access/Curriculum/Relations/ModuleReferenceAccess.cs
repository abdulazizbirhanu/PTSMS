using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PTSMSDAL.Access.Curriculum.Relations
{
    public class ModuleReferenceAccess
    {
        private PTSContext db = new PTSContext();

        public List<ModuleReference> List()
        {
            return db.ModuleReferences.ToList();
        }
        public List<ModuleReference> GetModuleReferencesByMuduleId(int moduleId)
        {
            return db.ModuleReferences.Where(mr => mr.ModuleId == moduleId).ToList();
        }
        public ModuleReference GetModuleReferencesByMuduleIdANdFileName(int moduleId, string fileName)
        {
            var references = db.ModuleReferences.Where(mr => mr.ModuleId == moduleId && mr.FileName == fileName).ToList();
            if (references.Count > 0)
                return references.FirstOrDefault();
            else
                return null;
        }

        public ModuleReference Details(int id)
        {
            try
            {
                ModuleReference moduleReference = db.ModuleReferences.Find(id);
                return moduleReference; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(ModuleReference moduleReference)
        {
            try
            {
                moduleReference.StartDate = DateTime.Now;
                moduleReference.EndDate = new DateTime(9999, 12, 31);
                moduleReference.CreationDate = DateTime.Now;
                moduleReference.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                moduleReference.RevisionDate = DateTime.Now;
                moduleReference.RevisedBy = HttpContext.Current.User.Identity.Name;

                db.ModuleReferences.Add(moduleReference);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(ModuleReference moduleReference)
        {
            try
            {
                db.Entry(moduleReference).State = EntityState.Modified;
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
                ModuleReference moduleReference = db.ModuleReferences.Find(id);
                if (moduleReference != null)
                {
                    db.ModuleReferences.Remove(moduleReference);
                    db.SaveChanges();
                    return true;
                }
                return false;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
    }
}