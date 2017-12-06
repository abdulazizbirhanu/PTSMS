using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling.Relations
{
    public class EquipmentScheduleBriefingDebriefingAccess
    {
        PTSContext db = new PTSContext();
        public List<EquipmentScheduleBriefingDebriefing> List()
        {
            return db.EquipmentScheduleBriefingDebriefings.Where(b=> b.Status != "Canceled").ToList();
        }

        public EquipmentScheduleBriefingDebriefing Details(int id)
        {
            try
            {
                EquipmentScheduleBriefingDebriefing equipmentScheduleBriefingDebriefing = db.EquipmentScheduleBriefingDebriefings.Where(b => b.Status != "Canceled" && b.EquipmentScheduleBriefingDebriefingId == id).FirstOrDefault();
                if (equipmentScheduleBriefingDebriefing == null)
                {
                    return null;
                }
                return equipmentScheduleBriefingDebriefing; // Success
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public EquipmentScheduleBriefingDebriefing GetEquipmentSchduleBriefingDebriefing(int fTDAndFlyingScheduleId, bool isBriefing, bool isDebriefing, List<EquipmentScheduleBriefingDebriefing> equipmentScheduleBriefingDebriefingList)
        {
            try
            {
                if (isBriefing)
                {
                    var result = equipmentScheduleBriefingDebriefingList.Where(b => b.FlyingFTDScheduleId == fTDAndFlyingScheduleId && b.BriefingAndDebriefing.IsBriefing == true && b.Status != "Canceled").ToList();
                    if (result.Count > 0)
                        return result.FirstOrDefault();
                }
                if (isDebriefing)
                {
                    var result = equipmentScheduleBriefingDebriefingList.Where(d => d.FlyingFTDScheduleId == fTDAndFlyingScheduleId && d.BriefingAndDebriefing.IsDebriefing == true && d.Status != "Canceled").ToList();
                    if (result.Count > 0)
                        return result.FirstOrDefault();
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public EquipmentScheduleBriefingDebriefing GetEquipmentSchduleBriefingDebriefing(int fTDAndFlyingScheduleId, bool isBriefing, bool isDebriefing)
        {
            try
            {
                var result = db.EquipmentScheduleBriefingDebriefings.Where(b => b.BriefingAndDebriefing.BriefingAndDebriefingId == fTDAndFlyingScheduleId && b.BriefingAndDebriefing.IsBriefing == isBriefing && b.BriefingAndDebriefing.IsDebriefing == isDebriefing && b.Status != "Canceled").ToList();
                if (result.Count > 0)
                    return result.FirstOrDefault();
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public List<EquipmentScheduleBriefingDebriefing> GetEquipmentSchduleBriefingDebriefingList(int fTDAndFlyingScheduleId, bool isBriefing, bool isDebriefing)
        {
            try
            {
                var result = db.EquipmentScheduleBriefingDebriefings.Where(b => b.BriefingAndDebriefingId == fTDAndFlyingScheduleId && b.BriefingAndDebriefing.IsBriefing == isBriefing && b.BriefingAndDebriefing.IsDebriefing == isDebriefing && b.Status != "Canceled").ToList();
                if (result.Count > 0)
                    return result;

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public EquipmentScheduleBriefingDebriefing GetEquipmentSchduleBriefingDebriefingByScheduleId(int fTDAndFlyingScheduleId, bool isBriefing, bool isDebriefing)
        {
            try
            {

                var result = db.EquipmentScheduleBriefingDebriefings.Where(b => b.FlyingFTDScheduleId == fTDAndFlyingScheduleId && b.BriefingAndDebriefing.IsBriefing == isBriefing && b.BriefingAndDebriefing.IsDebriefing == isDebriefing && b.Status != "Canceled").ToList();
                if (result.Count > 0)
                    return result.FirstOrDefault();

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public bool Add(EquipmentScheduleBriefingDebriefing equipmentScheduleBriefingDebriefing)
        {
            try
            {
                db.EquipmentScheduleBriefingDebriefings.Add(equipmentScheduleBriefingDebriefing);
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Revise(EquipmentScheduleBriefingDebriefing equipmentScheduleBriefingDebriefing)
        {
            try
            {
                db.Entry(equipmentScheduleBriefingDebriefing).State = EntityState.Modified;
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
                var equipmentScheduleBriefingDebriefing = db.EquipmentScheduleBriefingDebriefings.Where(b => b.Status != "Canceled" && b.EquipmentScheduleBriefingDebriefingId == id).FirstOrDefault();
                db.EquipmentScheduleBriefingDebriefings.Remove(equipmentScheduleBriefingDebriefing);

                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
