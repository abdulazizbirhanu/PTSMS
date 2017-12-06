using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Dispatch;
using PTSMSBAL.Dispatch;
using PTSMSBAL.Scheduling.Operations;
using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSBAL.Scheduling.Relations;
using Microsoft.AspNet.Identity.Owin;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSBAL.Scheduling.Others;
using PTSMSDAL.Models.Scheduling.Operations;
using System.Collections.Generic;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Dispatch.Master;
using PTSMS.Models;
using System;

namespace PTSMS.Controllers.Dispatch
{
    public class ActivityCheckInController : Controller
    {
        private PTSContext db = new PTSContext();
        ActivityCheckInLogic activityCheckInLogic = new ActivityCheckInLogic();
        AirportLogic airportLogic = new AirportLogic();
        InstructorLogic instructorLogic = new InstructorLogic();
        EquipmentLogic equipmentLogic = new EquipmentLogic();
        SchedulerUtility schedulerUtility = new SchedulerUtility();

        private ApplicationSignInManager _signInManager;
    
        private ApplicationDbContext _db = new ApplicationDbContext();
         
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

        // GET: ActivityCheckIn
        public ActionResult Index()
        {
            return View(activityCheckInLogic.List());
        }

        // GET: ActivityCheckIn/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityCheckIn activityCheckIn = activityCheckInLogic.Details((int)id);
            if (activityCheckIn == null)
            {
                return HttpNotFound();
            }
            return View(activityCheckIn);
        }

        // GET: ActivityCheckIn/Create
        public ActionResult Create()
        {
            ViewBag.ArrivalAirportId = new SelectList(airportLogic.List(), "AirportId", "AirportName");
            ViewBag.DepartureAirportId = new SelectList(airportLogic.List(), "AirportId", "AirportName");
            ViewBag.EquipmentId = new SelectList(equipmentLogic.List(), "EquipmentId", "NameOrSerialNo");
            ViewBag.FlyingFTDScheduleId = new SelectList(db.FlyingFTDSchedules, "FlyingFTDScheduleId", "Status");
            ViewBag.InstructorId = new SelectList(instructorLogic.List(), "InstructorId", "CreatedBy");
            ViewBag.ObserverId = new SelectList(instructorLogic.List(), "InstructorId", "CreatedBy");

            return View();
        }

        [HttpGet]
        public PartialViewResult CreateCheckIn(int flyingFTDScheduleId)
        {
            ActivityCheckIn activityCheckIn = activityCheckInLogic.CheckInDetailsByScheduleId(flyingFTDScheduleId);
            FlyingFTDSchedule flyingFTDSchedule = schedulerUtility.Details(flyingFTDScheduleId);
            ViewBag.CheckInTime = DateTime.Now;
            if (flyingFTDSchedule != null)
            {

                //Check whether the lesson is Flying or FTD
                if (flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                    ViewBag.IsFlyingLesson = false;
                else if (flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                    ViewBag.IsFlyingLesson = true;

                List<Airport> airportList = airportLogic.List();
                airportList.Add(new Airport { AirportId = 0, AirportName = "-- Select Arrival Airport --" });

                List<Instructor> instructorList = instructorLogic.List();
                instructorList.Add(new Instructor
                {
                    InstructorId = 0,
                    Person = new Person
                    {
                        FirstName = "-- Select Observer --",
                        MiddleName = ""
                    }
                });

                List<CheckInStatus> checkInStatusList = db.CheckInStatuss.ToList();
                checkInStatusList.Add(new CheckInStatus { CheckInStatusId = 0, CheckInStatusName = "-- Select Check-In Status --" });

                List<Destination> destinationList = db.Destinations.ToList();
                destinationList.Add(new Destination { DestinationId = 0, DestinationName = "-- Select Destination --" });

                List<ParkingSpot> parkingSpotList = db.ParkingSpots.ToList();
                parkingSpotList.Add(new ParkingSpot { ParkingSpotId = 0, ParkingSpotName = "-- Select Parking Spot --" });

                List<OperationArea> operationAreaList = db.OperationAreas.ToList();
                operationAreaList.Add(new OperationArea { OperationAreaId = 0, OperationAreaName = "-- Select Operation Area --" });


                if (activityCheckIn != null)
                {
                    ViewBag.CheckInTime = activityCheckIn.CheckInTime;
                    ViewBag.ArrivalAirportId = new SelectList(airportList.OrderBy(x => x.AirportId).Select(item => new
                    {
                        AirportId = item.AirportId > 0 ? item.AirportId.ToString() : "",
                        AirportName = item.AirportName
                    }), "AirportId", "AirportName", activityCheckIn.ArrivalAirportId);

                    ViewBag.DepartureAirportId = new SelectList(airportList.OrderBy(x => x.AirportId).Select(item => new
                    {
                        AirportId = item.AirportId > 0 ? item.AirportId.ToString() : "",
                        AirportName = item.AirportName
                    }), "AirportId", "AirportName", activityCheckIn.DepartureAirportId);
                    ViewBag.EquipmentId = new SelectList(equipmentLogic.List(), "EquipmentId", "NameOrSerialNo", activityCheckIn.EquipmentId);
                    ViewBag.InstructorId = new SelectList(instructorLogic.List().Select(item => new
                    {
                        InstructorId = item.InstructorId,
                        InstructorName = item.Person.FirstName + " " + item.Person.MiddleName
                    }), "InstructorId", "InstructorName", activityCheckIn.InstructorId);


                    ViewBag.ObserverId = new SelectList(instructorList.OrderBy(x => x.InstructorId).Select(item => new
                    {
                        InstructorId = item.InstructorId > 0 ? item.InstructorId.ToString() : "",
                        InstructorName = item.Person.FirstName + " " + item.Person.MiddleName
                    }), "InstructorId", "InstructorName", activityCheckIn.ObserverId);

                    ViewBag.CheckInStatusId = new SelectList(checkInStatusList.OrderBy(x => x.CheckInStatusId).Select(item => new
                    {
                        CheckInStatusId = item.CheckInStatusId > 0 ? item.CheckInStatusId.ToString() : "",
                        CheckInStatusName = item.CheckInStatusName
                    }), "CheckInStatusId", "CheckInStatusName", activityCheckIn.CheckInStatusId);

                    ViewBag.DestinationId = new SelectList(destinationList.OrderBy(x => x.DestinationId).Select(item => new
                    {
                        DestinationId = item.DestinationId > 0 ? item.DestinationId.ToString() : "",
                        DestinationName = item.DestinationName
                    }), "DestinationId", "DestinationName", activityCheckIn.DestinationId);
                    ViewBag.ParkingSpotId = new SelectList(parkingSpotList.OrderBy(x => x.ParkingSpotId).Select(item => new
                    {
                        ParkingSpotId = item.ParkingSpotId > 0 ? item.ParkingSpotId.ToString() : "",
                        ParkingSpotName = item.ParkingSpotName
                    }), "ParkingSpotId", "ParkingSpotName", activityCheckIn.ParkingSpotId);
                    ViewBag.OperationAreaId = new SelectList(operationAreaList.OrderBy(x => x.OperationAreaId).Select(item => new
                    {
                        OperationAreaId = item.OperationAreaId > 0 ? item.OperationAreaId.ToString() : "",
                        OperationAreaName = item.OperationAreaName
                    }), "OperationAreaId", "OperationAreaName", activityCheckIn.OperationAreaId);
                }
                else
                {
                    activityCheckIn = new ActivityCheckIn();
                    activityCheckIn.FlyingFTDScheduleId = flyingFTDScheduleId;
                    ViewBag.ArrivalAirportId = new SelectList(airportList.OrderBy(x => x.AirportId).Select(item => new
                    {
                        AirportId = item.AirportId > 0 ? item.AirportId.ToString() : "",
                        AirportName = item.AirportName
                    }), "AirportId", "AirportName");
                    ViewBag.DepartureAirportId = new SelectList(airportList.OrderBy(x => x.AirportId).Select(item => new
                    {
                        AirportId = item.AirportId > 0 ? item.AirportId.ToString() : "",
                        AirportName = item.AirportName
                    }), "AirportId", "AirportName");
                    ViewBag.EquipmentId = new SelectList(equipmentLogic.List(), "EquipmentId", "NameOrSerialNo", flyingFTDSchedule.EquipmentId);
                    ViewBag.InstructorId = new SelectList(instructorLogic.List().Select(item => new
                    {
                        InstructorId = item.InstructorId,
                        InstructorName = item.Person.FirstName + " " + item.Person.MiddleName
                    }), "InstructorId", "InstructorName", flyingFTDSchedule.InstructorId);
                    ViewBag.ObserverId = new SelectList(instructorList.OrderBy(x => x.InstructorId).Select(item => new
                    {
                        InstructorId = item.InstructorId > 0 ? item.InstructorId.ToString() : "",
                        InstructorName = item.Person.FirstName + " " + item.Person.MiddleName
                    }), "InstructorId", "InstructorName");

                    ViewBag.CheckInStatusId = new SelectList(checkInStatusList.OrderBy(x => x.CheckInStatusId).Select(item => new
                    {
                        CheckInStatusId = item.CheckInStatusId > 0 ? item.CheckInStatusId.ToString() : "",
                        CheckInStatusName = item.CheckInStatusName
                    }), "CheckInStatusId", "CheckInStatusName");
                    ViewBag.DestinationId = new SelectList(destinationList.OrderBy(x => x.DestinationId).Select(item => new
                    {
                        DestinationId = item.DestinationId > 0 ? item.DestinationId.ToString() : "",
                        DestinationName = item.DestinationName
                    }), "DestinationId", "DestinationName");
                    ViewBag.ParkingSpotId = new SelectList(parkingSpotList.OrderBy(x => x.ParkingSpotId).Select(item => new
                    {
                        ParkingSpotId = item.ParkingSpotId > 0 ? item.ParkingSpotId.ToString() : "",
                        ParkingSpotName = item.ParkingSpotName
                    }), "ParkingSpotId", "ParkingSpotName");
                    ViewBag.OperationAreaId = new SelectList(operationAreaList.OrderBy(x => x.OperationAreaId).Select(item => new
                    {
                        OperationAreaId = item.OperationAreaId > 0 ? item.OperationAreaId.ToString() : "",
                        OperationAreaName = item.OperationAreaName
                    }), "OperationAreaId", "OperationAreaName");
                }
            }
            return PartialView("CreateCheckIn", activityCheckIn);
        }

        // POST: ActivityCheckIn/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ActivityCheckIn activityCheckIn)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string password = Request.Form["txtActivityCheckInPassword"]; 
                var userValidationResult = SignInManager.PasswordSignInAsync(HttpContext.User.Identity.Name, password, false, shouldLockout: false);
  
                if (userValidationResult.Result.ToString() == "Success")
                {
                    if (ModelState.IsValid)
                    {
                        if (activityCheckInLogic.Add(activityCheckIn))
                            TempData["FTDAndFlyingScheduleMessage"] = "You have made a successful check-in.";
                        else
                            TempData["FTDAndFlyingScheduleMessage"] = "You have got failure while making check-in.";
                    }
                }
                else
                {
                    TempData["FTDAndFlyingScheduleMessage"] = "Failed to check-in due to incorrect password.";
                }
            }
            else
            {
                TempData["FTDAndFlyingScheduleMessage"] = "The password you entered is not the password of the active user.";
            }
            return RedirectToAction("EquipmentScheduler", "Scheduler", new { });
        }
        [HttpPost]
        public JsonResult CreateJson(int FlyingFTDScheduleId,int ActivityCheckInId,int EquipmentId,int InstructorId,string CheckInTime,int DestinationId,int DepartureAirportId,int ArrivalAirportId,int CheckInStatusId,string Comments)
        {
            bool isSuccess = false;
            ActivityCheckIn activityCheckIn = new ActivityCheckIn();
            
            if (ActivityCheckInId != 0)
              activityCheckIn = activityCheckInLogic.Details(ActivityCheckInId);

            activityCheckIn.FlyingFTDScheduleId = FlyingFTDScheduleId;
            activityCheckIn.EquipmentId = EquipmentId;
            activityCheckIn.InstructorId = InstructorId;
            if(!string.IsNullOrEmpty(CheckInTime))
            activityCheckIn.CheckInTime = Convert.ToDateTime(CheckInTime);
            activityCheckIn.DestinationId = DestinationId;
            activityCheckIn.ArrivalAirportId = ArrivalAirportId;
            activityCheckIn.CheckInStatusId = CheckInStatusId;
            activityCheckIn.DepartureAirportId = DepartureAirportId;
            activityCheckIn.Comments = Comments;
            //if (HttpContext.User.Identity.Name != null)
            //{
            //    string password = Request.Form["txtActivityCheckInPassword"];
            //    var userValidationResult = SignInManager.PasswordSignInAsync(HttpContext.User.Identity.Name, password, false, shouldLockout: false);

            //    if (userValidationResult.Result.ToString() == "Success")
            //    {
            //if (ModelState.IsValid)
            //{
            if (activityCheckInLogic.Add(activityCheckIn))
            {
                TempData["FTDAndFlyingScheduleMessage"] = "You have made a successful check-in.";
                isSuccess = true;
            }
            else
            {
                TempData["FTDAndFlyingScheduleMessage"] = "You have got failure while making check-in.";
                isSuccess = false;
            }
            //}
            //    }
            //    else
            //    {
            //        TempData["FTDAndFlyingScheduleMessage"] = "Failed to check-in due to incorrect password.";
            //    }
            //}
            //else
            //{
            //    TempData["FTDAndFlyingScheduleMessage"] = "The password you entered is not the password of the active user.";
            //}
            return Json(new { isSuccess = isSuccess, Message = TempData["FTDAndFlyingScheduleMessage"].ToString() }, JsonRequestBehavior.AllowGet);
        }
        // GET: ActivityCheckIn/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityCheckIn activityCheckIn = activityCheckInLogic.Details((int)id);
            if (activityCheckIn == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArrivalAirportId = new SelectList(airportLogic.List(), "AirportId", "AirportName", activityCheckIn.ArrivalAirportId);
            ViewBag.DepartureAirportId = new SelectList(airportLogic.List(), "AirportId", "AirportName", activityCheckIn.DepartureAirportId);
            ViewBag.EquipmentId = new SelectList(equipmentLogic.List(), "EquipmentId", "NameOrSerialNo", activityCheckIn.EquipmentId);
            ViewBag.FlyingFTDScheduleId = new SelectList(db.FlyingFTDSchedules, "FlyingFTDScheduleId", "Status", activityCheckIn.FlyingFTDScheduleId);
            ViewBag.InstructorId = new SelectList(instructorLogic.List(), "InstructorId", "CreatedBy", activityCheckIn.InstructorId);
            ViewBag.ObserverId = new SelectList(instructorLogic.List(), "InstructorId", "CreatedBy", activityCheckIn.ObserverId);
            return View(activityCheckIn);
        }

        // POST: ActivityCheckIn/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ActivityCheckInId,FlyingFTDScheduleId,EquipmentId,InstructorId,ObserverId,Sequence,StartTime,EndTime,DestinationId,ParkingSpotId,DepartureAirportId,ArrivalAirportId,Comments,StatusId")] ActivityCheckIn activityCheckIn)
        {
            if (ModelState.IsValid)
            {
                activityCheckInLogic.Revise(activityCheckIn);
                return RedirectToAction("Index");
            }
            ViewBag.ArrivalAirportId = new SelectList(airportLogic.List(), "AirportId", "AirportName", activityCheckIn.ArrivalAirportId);
            ViewBag.DepartureAirportId = new SelectList(airportLogic.List(), "AirportId", "AirportName", activityCheckIn.DepartureAirportId);
            ViewBag.EquipmentId = new SelectList(equipmentLogic.List(), "EquipmentId", "NameOrSerialNo", activityCheckIn.EquipmentId);
            ViewBag.FlyingFTDScheduleId = new SelectList(db.FlyingFTDSchedules, "FlyingFTDScheduleId", "Status", activityCheckIn.FlyingFTDScheduleId);
            ViewBag.InstructorId = new SelectList(instructorLogic.List(), "InstructorId", "CreatedBy", activityCheckIn.InstructorId);
            ViewBag.ObserverId = new SelectList(instructorLogic.List(), "InstructorId", "CreatedBy", activityCheckIn.ObserverId);
            return View(activityCheckIn);
        }

        // GET: ActivityCheckIn/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityCheckIn activityCheckIn = activityCheckInLogic.Details((int)id);
            if (activityCheckIn == null)
            {
                return HttpNotFound();
            }
            return View(activityCheckIn);
        }

        // POST: ActivityCheckIn/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            activityCheckInLogic.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
