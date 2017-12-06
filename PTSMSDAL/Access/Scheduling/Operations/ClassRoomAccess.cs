using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Operations;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class ClassRoomAccess
    {
        private PTSContext db = new PTSContext();

        public List<ClassRoom> List()
        {
            return db.ClassRooms.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public ClassRoom Details(int id)
        {
            try
            {
                ClassRoom classRoom = db.ClassRooms.Find(id);
                if (classRoom == null)
                {
                    return null; // Not Found
                }
                return classRoom; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(ClassRoom classRoom)
        {
            try
            {

                classRoom.StartDate = DateTime.Now;
                classRoom.EndDate = Constants.EndDate;
                classRoom.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                classRoom.CreationDate = DateTime.Now;

                db.ClassRooms.Add(classRoom);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(ClassRoom classRoom)
        {
            try
            {
                classRoom.RevisionDate = DateTime.Now;
                classRoom.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(classRoom).State = EntityState.Modified;
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
                ClassRoom classRoom = db.ClassRooms.Find(id);
                classRoom.EndDate = DateTime.Now;
                db.Entry(classRoom).State = EntityState.Modified;
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