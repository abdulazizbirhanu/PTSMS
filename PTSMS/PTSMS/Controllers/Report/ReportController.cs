using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using PTSMSDAL.Models.Report;

namespace PTSMS.Controllers.Report
{
    public class ReportController : Controller
    {
        SqlConnection conn = new SqlConnection("Data Source=svhqbsd01;Initial Catalog=PTSDB_PRODUCTION;User ID=ptsuser;Password=abcd@1234");
        //SqlConnection conn = new SqlConnection("Data Source=svhqbsd01;Initial Catalog=PTSDB_PRE_PRODUCTION;User ID=ptsuser;Password=abcd@1234");
        // GET: Report
        public ActionResult MonthlyFlightTimeReport()
        {
            return View();
        }
        public ActionResult ActiveStudentsReport()
        {
            return View();
        }
        public ActionResult ScheduleCancelationReport()
        {
            return View();
        }
        public ActionResult SignInAndOutReport()
        {
            return View();
        }
        public ActionResult ReportViewer()
        {
            return View();//1.	MONTHLY FLIGHT TIME REPORT
        }
        public ActionResult EquipmentUtilization()
        {

            return View();
        }
        public ActionResult InstructorUtilizationReport()
        {

            return View();
        }
        public ActionResult LessonReport()
        {

            return View();
        }
        public ActionResult ScheduleFlightsReport()
        {

            return View();
        }
        public ActionResult FlownFlightsReport()
        {

            return View();
        }
        public ActionResult CanceledFlightsReport()
        {

            return View();
        }
        public ActionResult InstructorFlownTimeReport()
        {

            return View();
        }
        public ActionResult AircraftFlownTimeReport()
        {

            return View();
        }
        public ActionResult InstructorAircraftReport()
        {

            return View();
        }
        public ActionResult InstructorLicenseType()
        {

            return View();
        }
        public ActionResult AircraftStatus()
        {

            return View();
        }
        [AllowAnonymous]
        public ActionResult ReportViewer(string returnUrl)
        {
            return View();
        }


        public ActionResult GenerateAndDisplayReport(FormCollection formCollection)
        {
            string ReportYear = Request.Form["ReportYear"];
            string ReportQuarter = Request.Form["ReportQuarter"];
            TempData["ReportYear"] = ReportYear;
            TempData["ReportQuarter"] = ReportQuarter;
            string FromDate = string.Empty;
            string ToDate = string.Empty;
            genereteDateOfQuarter(ReportYear, ReportQuarter, ref FromDate, ref ToDate);

            List<EqipmentReportView> equviewList = new List<EqipmentReportView>();
            conn.Open();
            SqlCommand command = new SqlCommand("Select NameOrSerialNo,EquipmentType,Model,IsMultiEngine,sum(Planned) as Planned,sum(Actual) as Actual,CountFlag from [VIEW_EQUIPMENT_UTILIZATION_BI] where Date between '" + FromDate + "' and '" + ToDate + "' GROUP BY NameOrSerialNo,EquipmentType,Model,IsMultiEngine,CountFlag", conn);
            DataTable dt = new DataTable();
            dt.Load(command.ExecuteReader());
            foreach (DataRow dr in dt.Rows)
            {
                EqipmentReportView equview = new EqipmentReportView();
                equview.NameOrSerialNo = dr["NameOrSerialNo"].ToString();
                equview.stringEquipmentType = dr["EquipmentType"].ToString();
                equview.Model = dr["Model"].ToString();
                equview.IsMultiEngine = dr["IsMultiEngine"].ToString();
                equview.Planned = dr["Planned"].ToString();
                equview.Actual = dr["Actual"].ToString();
                equview.CountFlg = dr["CountFlag"].ToString();
                equviewList.Add(equview);

            }
            ViewBag.equviewList = equviewList;
            return View("EquipmentUtilization");

        }
        public ActionResult GenerateAndDisplayInstuctorReport(FormCollection formCollection)
        {
            string ReportYear = Request.Form["ReportYear"];
            string ReportQuarter = Request.Form["ReportQuarter"];
            TempData["ReportYear"] = ReportYear;
            TempData["ReportQuarter"] = ReportQuarter;
            string FromDate = string.Empty;
            string ToDate = string.Empty;
            genereteDateOfQuarter(ReportYear, ReportQuarter, ref FromDate, ref ToDate);

            List<InstructerReportView> instructorsList = new List<InstructerReportView>();
            conn.Open();

            SqlCommand command = new SqlCommand("Select InstructorId,InstructorName,SUM((cast(EndingTime as int)-cast(StartingTime as int))) as workingHR from [VIEW_INSTRUCTOR_UTILIZATION_REP] where dt between '" + FromDate + "' and '" + ToDate + "' GROUP BY InstructorId,InstructorName", conn);
            DataTable dt = new DataTable();
            dt.Load(command.ExecuteReader());
            foreach (DataRow dr in dt.Rows)
            {
                InstructerReportView InsView = new InstructerReportView();
                InsView.InstructorId = dr["InstructorId"].ToString();
                InsView.InstructorName = dr["InstructorName"].ToString();
                InsView.WorkingHour = dr["workingHR"].ToString();
                instructorsList.Add(InsView);

            }
            ViewBag.instructorsList = instructorsList;

            return View("InstructorUtilizationReport");

        }
        public ActionResult GenerateAndDisplayLessonReport(FormCollection formCollection)
        {
            string FDate = Request.Form["FromDate"];
            string TDate = Request.Form["ToDate"];
            TempData["FromDate"] = FDate;
            TempData["ToDate"] = TDate;
            DateTime FromDate = new DateTime();
            DateTime ToDate = new DateTime();
            if (FDate != "")
                FromDate = Convert.ToDateTime(FDate);
            if (TDate != "")
                ToDate = Convert.ToDateTime(TDate);

            List<LessonReportBO> lessonsList = new List<LessonReportBO>();
            conn.Open();
            SqlCommand command = new SqlCommand("Select trFname,trLname,inFname,inLname,LessonName,NameOrSerialNo,ScheduleStartTime,ScheduleEndTime,BatchClassName,Status from [LESSON_REPORT] where ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "'", conn);
            DataTable dt = new DataTable();
            dt.Load(command.ExecuteReader());
            foreach (DataRow dr in dt.Rows)
            {
                LessonReportBO LessonView = new LessonReportBO();
                LessonView.trFname = dr["trFname"].ToString();
                LessonView.trLname = dr["trLname"].ToString();
                LessonView.inFname = dr["inFname"].ToString();
                LessonView.inLname = dr["inLname"].ToString();
                LessonView.LessonName = dr["LessonName"].ToString();
                LessonView.NameOrSerialNo = dr["NameOrSerialNo"].ToString();
                LessonView.ScheduleStartTime = dr["ScheduleStartTime"].ToString();
                LessonView.ScheduleEndTime = dr["ScheduleEndTime"].ToString();
                LessonView.BatchClassName = dr["BatchClassName"].ToString();
                LessonView.Status = dr["Status"].ToString();
                lessonsList.Add(LessonView);
            }
            ViewBag.lessonsList = lessonsList;
            return View("LessonReport");

        }
        public ActionResult GenerateAndDisplayScheduleLesson(FormCollection formCollection)
        {
            string ReportYear = Request.Form["ReportYear"];
            string ReportQuarter = Request.Form["ReportQuarter"];
            TempData["ReportYear"] = ReportYear;
            TempData["ReportQuarter"] = ReportQuarter;
            string FromDate = string.Empty;
            string ToDate = string.Empty;
            genereteDateOfQuarter(ReportYear, ReportQuarter, ref FromDate, ref ToDate);

            List<ScheduledFlights> ListScheduledFlights = new List<ScheduledFlights>();
            conn.Open();
            SqlCommand command = new SqlCommand("Select ACname,ScheduleStartTime,inFname,inLname,trFname,trLname,BatchClassName,Status,LessonName,LocationName,ScheduleEndTime from [SCHEDULED_FLIGHT] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "') and Status!='RampIn' and Status!='Evaluated' and Status!='Canceled'", conn);
            DataTable dt = new DataTable();
            dt.Load(command.ExecuteReader());
            foreach (DataRow dr in dt.Rows)
            {
                ScheduledFlights ScheduledFlight = new ScheduledFlights();
                ScheduledFlight.trFname = dr["trFname"].ToString();
                ScheduledFlight.trLname = dr["trLname"].ToString();
                ScheduledFlight.inFname = dr["inFname"].ToString();
                ScheduledFlight.inLname = dr["inLname"].ToString();
                ScheduledFlight.LessonName = dr["LessonName"].ToString();
                ScheduledFlight.NameOrSerialNo = dr["ACname"].ToString();
                ScheduledFlight.ScheduleStartTime = dr["ScheduleStartTime"].ToString();
                ScheduledFlight.ScheduleEndTime = dr["ScheduleEndTime"].ToString();
                ScheduledFlight.BatchClassName = dr["BatchClassName"].ToString();
                ScheduledFlight.Status = dr["Status"].ToString();
                ScheduledFlight.Location = dr["LocationName"].ToString();
                ListScheduledFlights.Add(ScheduledFlight);
            }
            ViewBag.ScheduledFlights = ListScheduledFlights;
            return View("ScheduleFlightsReport");

        }
        public ActionResult GenerateAndDisplayFlownFlight(FormCollection formCollection)
        {
            string ReportYear = Request.Form["ReportYear"];
            string ReportQuarter = Request.Form["ReportQuarter"];
            TempData["ReportYear"] = ReportYear;
            TempData["ReportQuarter"] = ReportQuarter;
            string FromDate = string.Empty;
            string ToDate = string.Empty;
            genereteDateOfQuarter(ReportYear, ReportQuarter, ref FromDate, ref ToDate);

            List<ScheduledFlights> ListScheduledFlights = new List<ScheduledFlights>();
            conn.Open();
            SqlCommand command = new SqlCommand("Select ACname,ScheduleStartTime,inFname,inLname,trFname,trLname,BatchClassName,Status,LessonName,LocationName,ScheduleEndTime from [SCHEDULED_FLIGHT] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "') and ( Status='RampIn' or Status='Evaluated')", conn);
            DataTable dt = new DataTable();
            dt.Load(command.ExecuteReader());
            foreach (DataRow dr in dt.Rows)
            {
                ScheduledFlights ScheduledFlight = new ScheduledFlights();
                ScheduledFlight.trFname = dr["trFname"].ToString();
                ScheduledFlight.trLname = dr["trLname"].ToString();
                ScheduledFlight.inFname = dr["inFname"].ToString();
                ScheduledFlight.inLname = dr["inLname"].ToString();
                ScheduledFlight.LessonName = dr["LessonName"].ToString();
                ScheduledFlight.NameOrSerialNo = dr["ACname"].ToString();
                ScheduledFlight.ScheduleStartTime = dr["ScheduleStartTime"].ToString();
                ScheduledFlight.ScheduleEndTime = dr["ScheduleEndTime"].ToString();
                ScheduledFlight.BatchClassName = dr["BatchClassName"].ToString();
                ScheduledFlight.Status = dr["Status"].ToString();
                ScheduledFlight.Location = dr["LocationName"].ToString();
                ListScheduledFlights.Add(ScheduledFlight);
            }
            ViewBag.ScheduledFlights = ListScheduledFlights;
            return View("FlownFlightsReport");

        }
        public ActionResult GenerateAndDisplayCanceledFlight(FormCollection formCollection)
        {
            string ReportYear = Request.Form["ReportYear"];
            string ReportQuarter = Request.Form["ReportQuarter"];
            TempData["ReportYear"] = ReportYear;
            TempData["ReportQuarter"] = ReportQuarter;
            string FromDate = string.Empty;
            string ToDate = string.Empty;
            genereteDateOfQuarter(ReportYear, ReportQuarter, ref FromDate, ref ToDate);

            List<ScheduledFlights> ListScheduledFlights = new List<ScheduledFlights>();
            conn.Open();
            SqlCommand command = new SqlCommand("Select ACname,ScheduleStartTime,inFname,inLname,trFname,trLname,BatchClassName,Status,LessonName,LocationName,ScheduleEndTime from [SCHEDULED_FLIGHT] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "') and Status='Canceled'", conn);
            DataTable dt = new DataTable();
            dt.Load(command.ExecuteReader());
            foreach (DataRow dr in dt.Rows)
            {
                ScheduledFlights ScheduledFlight = new ScheduledFlights();
                ScheduledFlight.trFname = dr["trFname"].ToString();
                ScheduledFlight.trLname = dr["trLname"].ToString();
                ScheduledFlight.inFname = dr["inFname"].ToString();
                ScheduledFlight.inLname = dr["inLname"].ToString();
                ScheduledFlight.LessonName = dr["LessonName"].ToString();
                ScheduledFlight.NameOrSerialNo = dr["ACname"].ToString();
                ScheduledFlight.ScheduleStartTime = dr["ScheduleStartTime"].ToString();
                ScheduledFlight.ScheduleEndTime = dr["ScheduleEndTime"].ToString();
                ScheduledFlight.BatchClassName = dr["BatchClassName"].ToString();
                ScheduledFlight.Status = dr["Status"].ToString();
                ScheduledFlight.Location = dr["LocationName"].ToString();
                ListScheduledFlights.Add(ScheduledFlight);
            }
            ViewBag.ScheduledFlights = ListScheduledFlights;
            return View("CanceledFlightsReport");

        }
        public ActionResult GenerateAndDisplayInsrtuctorFlownTime(FormCollection formCollection)
        {
            string FDate = Request.Form["FromDate"];
            string TDate = Request.Form["ToDate"];
            TempData["FromDate"] = FDate;
            TempData["ToDate"] = TDate;
            DateTime FromDate = new DateTime();
            DateTime ToDate = new DateTime();
            if (FDate != "")
                FromDate = Convert.ToDateTime(FDate);
            if (TDate != "")
                ToDate = Convert.ToDateTime(TDate);

            List<ScheduledFlights> ListInstructorFlownTime = new List<ScheduledFlights>();
            conn.Open();
            SqlCommand command = new SqlCommand("Select InstructorId,InstructorCompID, COUNT(ScheduleEndTime-ScheduleStartTime) TotalTime,inFname,inLname from [SCHEDULED_FLIGHT] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "') and Status!='RampIn' and Status!='Evaluated' and Status!='Canceled' group by InstructorId,InstructorCompID,inFname,inLname ", conn);
            SqlCommand commandFlown = new SqlCommand("Select InstructorId,InstructorCompID, COUNT(ScheduleEndTime-ScheduleStartTime) TotalTime,inFname,inLname  from [SCHEDULED_FLIGHT] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "')  and (Status='RampIn' or Status='Evaluated') group by InstructorId,InstructorCompID,inFname,inLname ", conn);
            SqlCommand commandCanceled = new SqlCommand("Select InstructorId,InstructorCompID, COUNT(ScheduleEndTime-ScheduleStartTime) TotalTime,inFname,inLname  from [SCHEDULED_FLIGHT] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "') and Status='Canceled' group by InstructorId,InstructorCompID,inFname,inLname ", conn);
            SqlCommand commandLicenseType = new SqlCommand("Select InstructorId,licenseType,  from [SCHEDULED_FLIGHT] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "') group by InstructorId,licenseType ", conn);
            DataTable dt = new DataTable();
            DataTable dtFlown = new DataTable();
            DataTable dtCanceled = new DataTable();
            DataTable dtLicenseType = new DataTable();
            dt.Load(command.ExecuteReader());
            dtFlown.Load(commandFlown.ExecuteReader());
            dtCanceled.Load(commandCanceled.ExecuteReader());
            dtLicenseType.Load(commandLicenseType.ExecuteReader());
            foreach (DataRow dr in dt.Rows)
            {
                DataRow drFlown = dtFlown.Select("InstructorId = '" + dr["InstructorId"].ToString() + "'").FirstOrDefault();
                DataRow drCanceled = dtCanceled.Select("InstructorId = '" + dr["InstructorId"].ToString() + "'").FirstOrDefault();
                List<DataRow> drLicenseType = dtLicenseType.Select("InstructorId = '" + dr["InstructorId"].ToString() + "'").ToList();

                ScheduledFlights InstructerReportView = new ScheduledFlights();
                InstructerReportView.instructorID = dr["InstructorCompID"].ToString();
                InstructerReportView.inFname = dr["inFname"].ToString();
                InstructerReportView.inLname = dr["inLname"].ToString(); ;
                InstructerReportView.totalTime = dr["TotalTime"].ToString();
                foreach (DataRow d in drLicenseType)
                {
                    if (InstructerReportView.licenseType != null)
                    {
                        InstructerReportView.licenseType = InstructerReportView.licenseType + ", " + d["licenseType"].ToString();
                        InstructerReportView.licenseDueDate = d["licenseDueDate"].ToString();
                    }
                    else
                        InstructerReportView.licenseType = d["licenseType"].ToString();
                }

                if (drFlown != null)
                    InstructerReportView.FlownTime = drFlown["TotalTime"].ToString();
                if (drCanceled != null)
                    InstructerReportView.CanceledTime = drCanceled["TotalTime"].ToString();
                ListInstructorFlownTime.Add(InstructerReportView);
            }
            ViewBag.ListInstructorFlownTime = ListInstructorFlownTime;
            return View("InstructorFlownTimeReport");

        }
        public ActionResult GenerateAndDisplayAircraftFlownTime(FormCollection formCollection)
        {
            string FDate = Request.Form["FromDate"];
            string TDate = Request.Form["ToDate"];
            TempData["FromDate"] = FDate;
            TempData["ToDate"] = TDate;
            DateTime FromDate = new DateTime();
            DateTime ToDate = new DateTime();
            if (FDate != "")
                FromDate = Convert.ToDateTime(FDate);
            if (TDate != "")
                ToDate = Convert.ToDateTime(TDate);

            List<AircraftFlownReport> ListAircraftFlownReport = new List<AircraftFlownReport>();
            conn.Open();
            SqlCommand commandForTotal = new SqlCommand("Select EquipmentId,NameOrSerialNo , COUNT(ScheduleEndTime-ScheduleStartTime) TotalTime from [AIRCRAFT_FLOWN_TIME] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "') and Status!='RampIn' and Status!='Evaluated' and Status!='Canceled' group by EquipmentId,NameOrSerialNo", conn);
            SqlCommand commandForFlown = new SqlCommand("Select EquipmentId,NameOrSerialNo , COUNT(ScheduleEndTime-ScheduleStartTime) FlownTime from [AIRCRAFT_FLOWN_TIME] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "')  and (Status='RampIn' or Status='Evaluated')  group by EquipmentId,NameOrSerialNo", conn);
            SqlCommand commandForCancled = new SqlCommand("Select EquipmentId,NameOrSerialNo , COUNT(ScheduleEndTime-ScheduleStartTime) CanceledTime from [AIRCRAFT_FLOWN_TIME] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "') and Status='Canceled'  group by EquipmentId,NameOrSerialNo", conn);
            DataTable dtTotal = new DataTable();
            DataTable dtFlown = new DataTable();
            DataTable dtCanceled = new DataTable();
            dtTotal.Load(commandForTotal.ExecuteReader());
            dtFlown.Load(commandForFlown.ExecuteReader());
            dtCanceled.Load(commandForCancled.ExecuteReader());
            foreach (DataRow dr in dtTotal.Rows)
            {
                AircraftFlownReport AircraftFlownReport = new AircraftFlownReport();
                AircraftFlownReport.EquipmentName = dr["NameOrSerialNo"].ToString();
                AircraftFlownReport.PlannedTime = dr["TotalTime"].ToString();
                AircraftFlownReport.EquipmentID = dr["EquipmentId"].ToString();

                DataRow drFlown = dtFlown.Select("EquipmentId = '" + dr["EquipmentId"].ToString() + "'").FirstOrDefault();
                DataRow drCanceled = dtCanceled.Select("EquipmentId = '" + dr["EquipmentId"].ToString() + "'").FirstOrDefault();

                try
                {
                    AircraftFlownReport.FlownTime = drFlown["FlownTime"].ToString();
                    AircraftFlownReport.CanceledTime = drCanceled["CanceledTime"].ToString();
                }
                catch (Exception)
                {
                }

                ListAircraftFlownReport.Add(AircraftFlownReport);
            }
            ViewBag.ListAircraftFlownReport = ListAircraftFlownReport;
            return View("AircraftFlownTimeReport");

        }
        public ActionResult GenerateAndDisplayInstructorFlownTimeOnAircraft(FormCollection formCollection)
        {
            string FDate = Request.Form["FromDate"];
            string TDate = Request.Form["ToDate"];
            TempData["FromDate"] = FDate;
            TempData["ToDate"] = TDate;
            DateTime FromDate = new DateTime();
            DateTime ToDate = new DateTime();
            if (FDate != "")
                FromDate = Convert.ToDateTime(FDate);
            if (TDate != "")
                ToDate = Convert.ToDateTime(TDate);

            List<ScheduledFlights> ListInstructorAircraftFlownReport = new List<ScheduledFlights>();
            conn.Open();
            SqlCommand commandFlown = new SqlCommand("Select InstructorId,InstructorCompID,ACname, COUNT(ScheduleEndTime-ScheduleStartTime) TotalTime,inFname,inLname from [SCHEDULED_FLIGHT] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "')  and (Status='RampIn' or Status='Evaluated') group by ACname,InstructorId,InstructorCompID,inFname,inLname", conn);

            DataTable dtFlown = new DataTable();
            dtFlown.Load(commandFlown.ExecuteReader());
            foreach (DataRow dr in dtFlown.Rows)
            {
                ScheduledFlights InstructorAircraft = new ScheduledFlights();
                InstructorAircraft.NameOrSerialNo = dr["ACname"].ToString();
                InstructorAircraft.FlownTime = dr["TotalTime"].ToString();
                InstructorAircraft.instructorID = dr["InstructorCompID"].ToString();
                InstructorAircraft.inFname = dr["inFname"].ToString();
                InstructorAircraft.inLname = dr["inLname"].ToString();
                ListInstructorAircraftFlownReport.Add(InstructorAircraft);
            }
            ViewBag.ListInstructorAircraftFlownReport = ListInstructorAircraftFlownReport;
            return View("InstructorAircraftReport");

        }
        public ActionResult GenerateAndDisplayInstructorTypeWithAircraftType(FormCollection formCollection)
        {
            string FDate = Request.Form["FromDate"];
            string TDate = Request.Form["ToDate"];
            TempData["FromDate"] = FDate;
            TempData["ToDate"] = TDate;
            DateTime FromDate = new DateTime();
            DateTime ToDate = new DateTime();
            if (FDate != "")
                FromDate = Convert.ToDateTime(FDate);
            if (TDate != "")
                ToDate = Convert.ToDateTime(TDate);

            List<ScheduledFlights> ListInstructorAircraftFlownReport = new List<ScheduledFlights>();
            conn.Open();
            SqlCommand commandFlown = new SqlCommand("Select InstructorId,InstructorCompID,ACname, COUNT(ScheduleEndTime-ScheduleStartTime) TotalTime,inFname,inLname from [SCHEDULED_FLIGHT] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "')  and (Status='RampIn' or Status='Evaluated') group by ACname,InstructorId,InstructorCompID,inFname,inLname", conn);

            DataTable dtFlown = new DataTable();
            dtFlown.Load(commandFlown.ExecuteReader());
            foreach (DataRow dr in dtFlown.Rows)
            {
                ScheduledFlights InstructorAircraft = new ScheduledFlights();
                InstructorAircraft.NameOrSerialNo = dr["ACname"].ToString();
                InstructorAircraft.FlownTime = dr["TotalTime"].ToString();
                InstructorAircraft.instructorID = dr["InstructorCompID"].ToString();
                InstructorAircraft.inFname = dr["inFname"].ToString();
                InstructorAircraft.inLname = dr["inLname"].ToString();
                ListInstructorAircraftFlownReport.Add(InstructorAircraft);
            }
            ViewBag.ListInstructorAircraftFlownReport = ListInstructorAircraftFlownReport;
            return View("InstructorAircraftReport");

        }
        public ActionResult GenerateAndDisplayInsrtuctorLicense(FormCollection formCollection)
        {
            string FDate = Request.Form["FromDate"];
            string TDate = Request.Form["ToDate"];
            TempData["FromDate"] = FDate;
            TempData["ToDate"] = TDate;
            DateTime FromDate = new DateTime();
            DateTime ToDate = new DateTime();
            if (FDate != "")
                FromDate = Convert.ToDateTime(FDate);
            if (TDate != "")
                ToDate = Convert.ToDateTime(TDate);

            List<ScheduledFlights> ListInstructorLicense = new List<ScheduledFlights>();
            conn.Open();
            SqlCommand commandLicenseType = new SqlCommand("Select InstructorId,InstructorCompID,inFname,inLname,licenseType,licenseDueDate from [SCHEDULED_FLIGHT] where (ScheduleStartTime between '" + FromDate + "' and '" + ToDate + "') group by InstructorId,InstructorCompID,inFname,inLname,licenseType,licenseDueDate ", conn);

            DataTable dtLicenseType = new DataTable();
            dtLicenseType.Load(commandLicenseType.ExecuteReader());
            foreach (DataRow dr in dtLicenseType.Rows)
            {

                ScheduledFlights InstructerReportView = new ScheduledFlights();
                InstructerReportView.instructorID = dr["InstructorCompID"].ToString();
                InstructerReportView.licenseType = dr["licenseType"].ToString();
                InstructerReportView.licenseDueDate = dr["licenseDueDate"].ToString();
                InstructerReportView.inFname = dr["inFname"].ToString();
                InstructerReportView.inLname = dr["inLname"].ToString();
                ListInstructorLicense.Add(InstructerReportView);
            }
            ViewBag.ListInstructorLicense = ListInstructorLicense;
            return View("InstructorLicenseType");

        }
        public ActionResult GenerateAndDisplayAircraftStatus(FormCollection formCollection)
        {
            string FDate = Request.Form["FromDate"];
            string TDate = Request.Form["ToDate"];
            TempData["FromDate"] = FDate;
            TempData["ToDate"] = TDate;
            DateTime FromDate = new DateTime();
            DateTime ToDate = new DateTime();
            if (FDate != "")
                FromDate = Convert.ToDateTime(FDate);
            if (TDate != "")
                ToDate = Convert.ToDateTime(TDate);

            List<ScheduledFlights> ListEquStatusView = new List<ScheduledFlights>();
            conn.Open();
            SqlCommand commandLicenseType = new SqlCommand("Select ACname,EquipmentStatusName from [SCHEDULED_FLIGHT]  group by ACname,EquipmentStatusName ", conn);

            DataTable dtLicenseType = new DataTable();
            dtLicenseType.Load(commandLicenseType.ExecuteReader());
            foreach (DataRow dr in dtLicenseType.Rows)
            {

                ScheduledFlights EquStatusView = new ScheduledFlights();
                EquStatusView.NameOrSerialNo = dr["ACname"].ToString();
                EquStatusView.equipmentStatus = dr["EquipmentStatusName"].ToString();
                ListEquStatusView.Add(EquStatusView);
            }
            ViewBag.ListEquStatusView = ListEquStatusView;
            return View("AircraftStatus");

        }
        public void genereteDateOfQuarter(string ReportYear, string ReportQuarter, ref string FromDate, ref string ToDate)
        {
            switch (ReportQuarter)
            {
                case "1":
                    FromDate = "1/1/" + ReportYear;
                    ToDate = "3/31/" + ReportYear;
                    break;
                case "2":
                    FromDate = "4/1/" + ReportYear;
                    ToDate = "6/30/" + ReportYear;
                    break;
                case "3":
                    FromDate = "7/1/" + ReportYear;
                    ToDate = "9/30/" + ReportYear;
                    break;
                case "4":
                    FromDate = "10/1/" + ReportYear;
                    ToDate = "12/31/" + ReportYear;
                    break;

            }

        }
    }

}