using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Operations;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.References;
using System;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Access.Enrollment.Operations
{
    public class InstructorAccess
    {
        private PTSContext db = new PTSContext();

        public List<Instructor> List()
        {
            return db.Instructors.Where(ins=>ins.EndDate > DateTime.Now).Include(p=>p.Person).ToList();
        }

        public Instructor Details(int id)
        {
            try
            {
                Instructor instructor = db.Instructors.Find(id);
                if (instructor == null)
                {
                    return null; // Not Found
                }
                return instructor; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public List<QualificationType> GetQulaificationType()
        {
            try
            {
                List<QualificationType> QualificationType = db.QualificationTypes.ToList();
                if (QualificationType == null)
                {
                    return null; // Not Found
                }
                return QualificationType; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(Instructor instructor, List<InstructorQualification> instructorQualification)
        {
            try
            {
                instructor.StartDate = DateTime.Now;
                instructor.EndDate = Constants.EndDate;
                instructor.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                instructor.CreationDate = DateTime.Now;

                db.Instructors.Add(instructor);
                if (db.SaveChanges() > 0)
                {
                    int instructorid = GetCurrentInstructorId();
                    foreach (var item in instructorQualification)
                    {
                        item.StartDate = DateTime.Now;
                        item.EndDate = Constants.EndDate;
                        item.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        item.CreationDate = DateTime.Now;
                        item.InstructorId = instructorid;
                        db.InstructorQualifications.Add(item);
                        db.SaveChanges();
                    }
                    return true;
                }
                return false; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(Instructor instructor)
        {
            try
            {
                db.Entry(instructor).State = EntityState.Modified;
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
                Instructor instructor = db.Instructors.Find(id);
                db.Instructors.Remove(instructor);
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public int GetCurrentInstructorId()
        {
            return Convert.ToInt32(db.Database.SqlQuery<decimal>("Select IDENT_CURRENT ('INSTRUCTOR')", new object[0]).FirstOrDefault());
        }
    }
}