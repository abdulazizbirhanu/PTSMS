using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Operations;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace PTSMSDAL.Access.Enrollment.Operations
{
    public class PersonDocumentAccess
    {
        public List<PersonDocument> List()
        {
            PTSContext db = new PTSContext();
            return db.PersonDocuments.ToList();
        }

        public PersonDocument Details(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                PersonDocument personDocument = db.PersonDocuments.Find(id);
                if (personDocument == null)
                {
                    return null; // Not Found
                }
                return personDocument; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public object Add(PersonDocument personDocument)
        {
            try
            {
                PTSContext db = new PTSContext();
                db.PersonDocuments.Add(personDocument);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(PersonDocument personDocument)
        {
            try
            {
                PTSContext db = new PTSContext();
                db.Entry(personDocument).State = EntityState.Modified;
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
                PersonDocument personDocument = db.PersonDocuments.Find(id);
                db.PersonDocuments.Remove(personDocument);
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