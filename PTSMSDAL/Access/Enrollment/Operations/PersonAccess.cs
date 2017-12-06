using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Operations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Enrollment.Operations
{
    public class PersonAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.Persons.ToList();
        }
        public object Details(int id)
        {
            try
            {
                Person person = db.Persons.Find(id);
                if (person == null)
                {
                    return false; // Not Found
                }
                return person; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public Person PDetails(int id)
        {
         
            try
            {
                Person person = db.Persons.Find(id);
                if (person == null)
                {
                    return null; // Not Found
                }
                return person; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }
        public object PersonDetail(string companyId)
        {
            try
            {
                var result = db.Persons.Where(p => p.CompanyId == companyId).ToList();


                if (result.Count() == 0)
                {
                    return null; // Not Found
                }
                return result.First(); // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public Person PersonDetailperson(string companyId)
        {
            Person person = new Person();
            try
            {
                List<Person> result = db.Persons.Where(p => p.CompanyId == companyId).ToList();


                if (result.Count() == 0)
                {
                    return null; // Not Found
                }
                person= result.First(); // Success
                return person;
            }
            catch (System.Exception e)
            {
                return person; // Exception
            }
        }
        public object Add(Person person)
        {
            try
            {
                person.StartDate = DateTime.Now;
                person.EndDate = DateTime.MaxValue;
                person.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                person.CreationDate = DateTime.Now;
                db.Persons.Add(person);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(Person person)
        {
            try
            {
                db.Entry(person).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false;  
            }
        }

        public object Delete(int id)
        {
            try
            {
                Person person = db.Persons.Find(id);
                db.Persons.Remove(person);
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