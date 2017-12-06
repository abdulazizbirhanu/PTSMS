using PTSMSDAL.Context;
using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Dispatch
{



    public class OverallGradeUpdateRequestAccess
    {
        PTSContext db = new PTSContext();
        public List<OverallGradeUpdateRequest> List()
        {
            return db.OverallGradeUpdateRequests.ToList();
        }

        public OverallGradeUpdateRequest Details(int id)
        {
            try
            {
                return db.OverallGradeUpdateRequests.Find(id);
            }
            catch (Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(OverallGradeUpdateRequest overallGradeUpdateRequest)
        {
            try
            {
                db.OverallGradeUpdateRequests.Add(overallGradeUpdateRequest);
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool ReviseAdd(OverallGradeUpdateRequest overallGradeUpdateRequestDetail)
        {
            try
            {
                db.Entry(overallGradeUpdateRequestDetail).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public OverallGradeUpdateRequest GetActivityRampOutDetailsByCheckInId(int overallGradeUpdateRequestId)
        {
            try
            {
                return db.OverallGradeUpdateRequests.Where(R => R.OverallGradeUpdateRequestId == overallGradeUpdateRequestId).ToList().FirstOrDefault();
            }
            catch (System.Exception e)
            {
                return new OverallGradeUpdateRequest();
            }
        }

        public bool Revise(OverallGradeUpdateRequest overallGradeUpdateRequest)
        {
            try
            {
                db.Entry(overallGradeUpdateRequest).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                OverallGradeUpdateRequest overallGradeUpdateRequest = db.OverallGradeUpdateRequests.Find(id);
                db.Entry(overallGradeUpdateRequest).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public OverallGradeUpdateRequest OverallGradeUpdateRequestDetails(OverallGradeUpdateRequest overallGradeUpdateRequest)
        {
            try
            {
                string statusName = Enum.GetName(typeof(OverallGradeUpdateRequestStatus), (int)OverallGradeUpdateRequestStatus.Requested);
                return db.OverallGradeUpdateRequests.Where(OG => OG.FlyingFTDScheduleId == overallGradeUpdateRequest.FlyingFTDScheduleId && OG.NewOverallGradeId == overallGradeUpdateRequest.NewOverallGradeId && OG.Status == statusName).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
