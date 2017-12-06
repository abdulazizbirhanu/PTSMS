using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.References;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Enrollment.References
{
    public class DocumentAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.Documents.ToList();
        }

        public object Details(int id)
        {
            try
            {
                DocumentType document = db.Documents.Find(id);
                if (document == null)
                {
                    return false; // Not Found
                }
                return document; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Add(DocumentType document)
        {
            try
            {
                db.Documents.Add(document);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(DocumentType document)
        {
            try
            {
                db.Entry(document).State = EntityState.Modified;
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
                DocumentType document = db.Documents.Find(id);
                db.Documents.Remove(document);
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