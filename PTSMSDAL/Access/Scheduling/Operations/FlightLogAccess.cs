using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class FlightLogAccess
    {
        PTSContext db = new PTSContext();
        public List<FlightLog> List()
        {
            return db.FlightLogs.ToList();
        }

        public FlightLog Details(int id)
        {
            try
            {
                FlightLog flightLog = db.FlightLogs.Find(id);
                if (flightLog == null)
                {
                    return null;
                }
                return flightLog;
            }
            catch (System.Exception e)
            {
                return null;
            }
        }
        public FlightLog FlightLogDetails(int activityCheckInId)
        {
            try
            {
                var flightLogs = db.FlightLogs.Where(fl => fl.ActivityCheckInId == activityCheckInId).ToList();
                if (flightLogs.Count > 0)
                {
                    return flightLogs.FirstOrDefault();
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool Add(FlightLog flightLog)
        {
            try
            {
                db.FlightLogs.Add(flightLog);
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public bool Revise(FlightLog flightLog)
        {
            try
            {
                db.Entry(flightLog).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                FlightLog flightLog = db.FlightLogs.Find(id);
                db.FlightLogs.Remove(flightLog);
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
