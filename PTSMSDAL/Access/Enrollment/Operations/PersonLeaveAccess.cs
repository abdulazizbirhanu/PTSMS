using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Enrollment.Operations;

namespace PTSMSDAL.Access.Enrollment.Operations
{
    public class PersonLeaveAccess
    {
        public List<PersonLeave> List()
        {
            PTSContext db = new PTSContext();
            return db.PersonLeaves.Where(PL => PL.EndDate >= DateTime.Now).ToList();
        }

        public PersonLeave Details(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                PersonLeave personLeave = db.PersonLeaves.Find(id);
                if (personLeave == null)
                {
                    return null; // Not Found
                }
                return personLeave; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public List<PersonLeave> InstructorLeaves(int InstructorId)
        {
            PTSContext db = new PTSContext();
            var pesonLeave = db.PersonLeaves.Where(pl => pl.PersonId == InstructorId).ToList();
            if (pesonLeave.Count() > 0)
                return pesonLeave;
            return new List<PersonLeave>();
        }
        public List<PersonLeave> LeavesWithDate(int InstructorId, DateTime startingDate)
        {
            PTSContext db = new PTSContext();
            var pesonLeave = db.PersonLeaves.Where(pl => pl.PersonId == InstructorId && (pl.ToDate >= startingDate && pl.FromDate <= startingDate)).ToList();
            if (pesonLeave.Count() > 0)
                return pesonLeave;
            return new List<PersonLeave>();
        }
        public List<PersonLeave> GetLeave(int personId)
        {
            try
            {
                PTSContext db = new PTSContext();
                return db.PersonLeaves.Where(PL => PL.EndDate >= DateTime.Now && PL.PersonId == personId).ToList();
            }
            catch (Exception ex)
            {
                return new List<PersonLeave>();
            }

        }
        public List<PersonLeave> GetLeave(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                PTSContext db = new PTSContext();
                return db.PersonLeaves.Where(PL => PL.FromDate <= FromDate && PL.ToDate >= ToDate).ToList();
            }
            catch (Exception ex)
            {
                return new List<PersonLeave>();
            }

        }

        public object Add(PersonLeave personLeave)
        {
            try
            {
                PTSContext db = new PTSContext();
                personLeave.StartDate = DateTime.Now;
                personLeave.EndDate = Constants.EndDate;
                personLeave.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                personLeave.CreationDate = DateTime.Now;
                db.PersonLeaves.Add(personLeave);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(PersonLeave personLeave)
        {
            try
            {
                PTSContext db = new PTSContext();
                personLeave.RevisionDate = DateTime.Now;
                personLeave.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(personLeave).State = EntityState.Modified;
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
                PersonLeave personLeave = db.PersonLeaves.Find(id);
                db.PersonLeaves.Remove(personLeave);
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
