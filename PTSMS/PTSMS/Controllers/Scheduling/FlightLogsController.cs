using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSBAL.Scheduling.Operations;
using PTSMSBAL.Scheduling.Relations;
using PTSMSDAL.Models.Dispatch;
using PTSMSBAL.Dispatch;
using PTSMSBAL.Scheduling.Others;
using PTSMSDAL.Access.Scheduling.Operations;
using System;

namespace PTSMS.Controllers.Scheduling
{
    public class FlightLogsController : Controller
    {
        private ApplicationSignInManager _signInManager;

        private PTSContext db = new PTSContext();

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        AirportLogic airportLogic = new AirportLogic();
        EquipmentLogic equipmentLogic = new EquipmentLogic();

        FlightLogLogic flightLogLogic = new FlightLogLogic();
        ActivityCheckInLogic activityCheckInLogic = new ActivityCheckInLogic();

        // GET: FlightLogs
        public ActionResult Index()
        {
            return View(flightLogLogic.List());
        }

        [HttpGet]
        public PartialViewResult CreateFlightLogPartialView(int flyingFTDScheduleId)
        {
            FlightLog flightLog = new FlightLog();
            FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();
            FlyingFTDSchedule flyingFTDSchedule = fTDAndFlyingSchedulerAccess.Details(flyingFTDScheduleId);
            ViewBag.IsLessonEvaluated = true;

            if (flyingFTDSchedule != null)
            {
                if (flyingFTDSchedule.Status == Enum.GetName(typeof(FlyingFTDScheduleStatus), FlyingFTDScheduleStatus.Evaluated))
                {

                    ActivityCheckIn activityCheckIn = activityCheckInLogic.CheckInDetailsByScheduleId(flyingFTDScheduleId);

                    if (activityCheckIn != null)
                    {
                        flightLog = flightLogLogic.FlightLogDetails(activityCheckIn.ActivityCheckInId);
                        if (flightLog != null)
                            ViewBag.InstrumentApproachId = new SelectList(db.InstrumentApproachs.ToList(), "InstrumentApproachId", "InstrumentApproachName", flightLog.InstrumentApproachId);
                        else
                        {
                            ViewBag.InstrumentApproachId = new SelectList(db.InstrumentApproachs.ToList(), "InstrumentApproachId", "InstrumentApproachName");
                            flightLog = new FlightLog();
                            flightLog.ActivityCheckInId = activityCheckIn.ActivityCheckInId;
                        }
                    }
                    else
                    {
                        ViewBag.InstrumentApproachId = new SelectList(db.InstrumentApproachs.ToList(), "InstrumentApproachId", "InstrumentApproachName");
                    }
                }
                else
                    ViewBag.IsLessonEvaluated = false;
            }
            return PartialView("CreateFlightLogPartialView", flightLog);
        }



        [HttpGet]
        public PartialViewResult FlightLogDetail(int flyingFTDScheduleId)
        {
            FlightLog flightLog = new FlightLog();

            ActivityCheckIn activityCheckIn = activityCheckInLogic.CheckInDetailsByScheduleId(flyingFTDScheduleId);
            if (activityCheckIn != null)
            {
                var flightLogDetial = flightLogLogic.FlightLogDetails(activityCheckIn.ActivityCheckInId);
                if (flightLogDetial != null)
                {
                    flightLog = flightLogDetial;
                    ViewBag.IsFlightLogRegistered = true;
                }
                else
                {
                    ViewBag.IsFlightLogRegistered = false;
                }
            }
            else
            {
                ViewBag.IsFlightLogRegistered = false;
            }
            return PartialView("FlightLogDetail", flightLog);
        }
        // GET: FlightLogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightLog flightLog = db.FlightLogs.Find(id);
            if (flightLog == null)
            {
                return HttpNotFound();
            }
            return View(flightLog);
        }

        // GET: FlightLogs/Create
        public ActionResult Create()
        {
            ViewBag.ArrivalAirportId = new SelectList(db.Airports, "AirportId", "AirportName");
            ViewBag.DepartureAirportId = new SelectList(db.Airports, "AirportId", "AirportName");
            ViewBag.TailNoId = new SelectList(db.Equipments, "EquipmentId", "NameOrSerialNo");
            ViewBag.EquipmentTypeId = new SelectList(db.EquipmentModels, "EquipmentModelId", "EquipmentModelName");
            ViewBag.FlyingFTDScheduleId = new SelectList(db.FlyingFTDSchedules, "FlyingFTDScheduleId", "Status");
            return View();
        }

        // POST: FlightLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FlightLog flightLog)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string password = Request.Form["txtFlightLogPassword"];

                var userValidationResult = SignInManager.PasswordSignInAsync(HttpContext.User.Identity.Name, password, false, shouldLockout: false);

                if (userValidationResult.Result.ToString() == "Success")
                {
                    if (flightLogLogic.Add(flightLog))
                    {
                        TempData["FTDAndFlyingScheduleMessage"] = "Flight log has been saved successfully.";
                    }
                    else
                    {
                        TempData["FTDAndFlyingScheduleMessage"] = "Failed to save flight log.";
                    }
                }
                else
                {
                    TempData["FTDAndFlyingScheduleMessage"] = "Failed to save flight log due to incorrect password.";
                }
            }
            else
            {
                TempData["FTDAndFlyingScheduleMessage"] = "The password you entered is no the password of the current active user.";
            }
            return RedirectToAction("Equipment", "InstructorSchedule", null);
        }
        [HttpPost]
        public JsonResult CreateJson(int ActivityCheckInId, int DayLanding,int DayTakeOff,int FlightLogId,int InstrumentApproachId,int NightLanding,int NightTakeOff, string Remark)
        {
            bool isSuccess = false;
            FlightLog flightLog = new FlightLog();
            flightLog.ActivityCheckInId = ActivityCheckInId;
            flightLog.DayLanding = DayLanding;
            flightLog.DayTakeOff = DayTakeOff;
            flightLog.FlightLogId = FlightLogId;
            flightLog.InstrumentApproachId = InstrumentApproachId;
            flightLog.NightLanding = NightLanding;
            flightLog.NightTakeOff = NightTakeOff;
            flightLog.Remark = Remark;

            if (flightLogLogic.Add(flightLog))
            {
                TempData["FTDAndFlyingScheduleMessage"] = "Flight log has been saved successfully.";
                isSuccess = true;
            }
            else
            {
                TempData["FTDAndFlyingScheduleMessage"] = "Failed to save flight log.";
                isSuccess = false;
            }

            return Json(new { isSuccess = isSuccess, Message = TempData["FTDAndFlyingScheduleMessage"].ToString() }, JsonRequestBehavior.AllowGet);
        }

        // GET: FlightLogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightLog flightLog = db.FlightLogs.Find(id);
            if (flightLog == null)
            {
                return HttpNotFound();
            }
            ViewBag.InstrumentApproachId = new SelectList(db.InstrumentApproachs.ToList(), "InstrumentApproachId", "InstrumentApproachName", flightLog.InstrumentApproachId);
            return View(flightLog);
        }

        // POST: FlightLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FlightLog flightLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(flightLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View(flightLog);
        }

        // GET: FlightLogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightLog flightLog = db.FlightLogs.Find(id);
            if (flightLog == null)
            {
                return HttpNotFound();
            }
            return View(flightLog);
        }

        // POST: FlightLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FlightLog flightLog = db.FlightLogs.Find(id);
            db.FlightLogs.Remove(flightLog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
