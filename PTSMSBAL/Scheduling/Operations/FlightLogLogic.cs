using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Scheduling.Operations
{
    public class FlightLogLogic
    {
        FlightLogAccess flightLogAccess = new FlightLogAccess();

        public List<FlightLog> List()
        {
            return flightLogAccess.List();
        }

        public FlightLog Details(int id)
        {
            return flightLogAccess.Details(id);
        }
        public FlightLog FlightLogDetails(int activityCheckInId)
        {
            return flightLogAccess.FlightLogDetails(activityCheckInId);
        }

        public bool Add(FlightLog flightLog)
        {
            var flightLogDetail = flightLogAccess.FlightLogDetails(flightLog.ActivityCheckInId);
            if (flightLogDetail == null)
                return flightLogAccess.Add(flightLog);
            else
            {
                //flightLogDetail.FlyingFTDScheduleId = flightLog.FlyingFTDScheduleId;
                //flightLogDetail.Date = flightLog.Date;
                //flightLogDetail.TailNoId = flightLog.TailNoId;
                //flightLogDetail.ArrivalAirportId = flightLog.ArrivalAirportId;
                //flightLogDetail.DepartureAirportId = flightLog.DepartureAirportId;
                //flightLogDetail.ArrivalTime = flightLog.ArrivalTime;
                //flightLogDetail.DepartureTime = flightLog.DepartureTime;

                //flightLogDetail.PilotInCommand = flightLog.PilotInCommand;
                //flightLogDetail.TotalFlightTime = flightLog.TotalFlightTime;
                //flightLogDetail.Dual = flightLog.Dual;
                ////flightLogDetail.FlightEngineerFlightTime = flightLog.FlightEngineerFlightTime;
                //flightLogDetail.FlightTimeDay = flightLog.FlightTimeDay;
                //flightLogDetail.FlightTimeNight = flightLog.FlightTimeNight;
                //flightLogDetail.InstrumentFlightTime = flightLog.InstrumentFlightTime;
                //flightLogDetail.SyntheticTrainerTime = flightLog.SyntheticTrainerTime;
                flightLogDetail.ActivityCheckInId = flightLog.ActivityCheckInId;
                flightLogDetail.DayTakeOff = flightLog.DayTakeOff;
                flightLogDetail.NightTakeOff = flightLog.NightTakeOff;
                flightLogDetail.DayLanding = flightLog.DayLanding;
                flightLogDetail.NightLanding = flightLog.NightLanding;
                flightLogDetail.InstrumentApproach = flightLog.InstrumentApproach;
                flightLogDetail.Remark = flightLog.Remark;

                return flightLogAccess.Revise(flightLogDetail);
            }
        }

        public bool Revise(FlightLog flightLog)
        {
            return flightLogAccess.Revise(flightLog);
        }

        public bool Delete(int id)
        {
            return flightLogAccess.Delete(id);
        }
    }
}
