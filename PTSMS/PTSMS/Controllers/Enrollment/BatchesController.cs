using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using PTSMS.Models;
using PTSMSBAL.Curriculum.References;
using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSBAL.Logic.Scheduling.Operations;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Others.View;
using PTSMSDAL.Models.Scheduling.Operations;

namespace PTSMS.Controllers
{
    public class BatchesController : Controller
    {
        BatchLogic batchLogic = new BatchLogic();
        ProgramLogic programLogic = new ProgramLogic();
        DayTemplateLogic dayTemplateLogic = new DayTemplateLogic();
        PeriodTemplateLogic periodTemplateLogic = new PeriodTemplateLogic();
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _db = new ApplicationDbContext();
        public async System.Threading.Tasks.Task<JsonResult> EnrollTraineeAsync(int BatchId, string TraineeId)
        {
            if (!(string.IsNullOrEmpty(TraineeId) && string.IsNullOrWhiteSpace(TraineeId)))
            {
                List<resultSet> resultSet = new List<resultSet>();
                string[] TraineeIdArray = TraineeId.Split(',');
                List<string> TraineeIdList = TraineeIdArray.ToList();
                foreach (string traineeId in TraineeIdList)
                {
                    PersonAccess personeAccess = new PersonAccess();
                    Person person = personeAccess.PersonDetailperson(traineeId);
                    if (!(string.IsNullOrEmpty(traineeId) && string.IsNullOrWhiteSpace(traineeId)))
                    {
                        List<resultSet> result = batchLogic.EnrollTrainee(BatchId, Convert.ToInt32(person.PersonId), person.CompanyId, ref resultSet);

                        //create account for trainee here
                        var appContext = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
                        try
                        {
                            //Append missed ZEROS               
                            string userName = person.CompanyId;
                            string appendableDigit = "";
                            for (int i = 0; i < (8 - userName.Length); i++)
                                appendableDigit += "0";
                            if (string.IsNullOrEmpty(person.Email))
                                person.Email = person.CompanyId+"@email.com";
                            userName = appendableDigit + userName;
                            //
                            var user = new ApplicationUser { UserName = userName, Email = person.Email };

                            var addResult = await UserManager.CreateAsync(user, "Abcd@1234");
                            if (addResult.Succeeded)
                            {
                                if (resultSet.Count > 0)
                                {
                                    resultSet resultc = resultSet.Where(r => r.resultType == "AccountSuccess").FirstOrDefault();
                                    if (resultc != null)
                                        resultc.resultValue = resultc.resultValue + "," + traineeId;
                                    else
                                    {
                                        resultSet result1 = new resultSet();
                                        result1.resultType = "AccountSuccess";
                                        result1.resultValue = traineeId.ToString();
                                        resultSet.Add(result1);
                                    }
                                }
                                else
                                {
                                    resultSet resultc = new resultSet();
                                    resultc.resultType = "AccountSuccess";
                                    resultc.resultValue = traineeId.ToString();
                                    resultSet.Add(resultc);
                                }
                                _db.AddUserToRole(UserManager, user.Id, "Trainee");
                            }
                            else
                            {
                                if (resultSet.Count > 0)
                                {
                                    resultSet resultc = resultSet.Where(r => r.resultType == "AccountFailed").FirstOrDefault();
                                    if (resultc != null)
                                        resultc.resultValue = resultc.resultValue + "," + traineeId;
                                    else
                                    {
                                        resultSet result1 = new resultSet();
                                        foreach(string t in addResult.Errors)
                                        result1.resultType = result1.resultType + t;
                                        result1.resultValue = traineeId.ToString();
                                        resultSet.Add(result1);
                                    }
                                }
                                else
                                {
                                    resultSet resultc = new resultSet();
                                    resultc.resultType = addResult.Errors.ToString();
                                    resultc.resultValue = traineeId.ToString();
                                    resultSet.Add(resultc);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (resultSet != null)
                            {
                                resultSet resultc = resultSet.Where(r => r.resultType == "AccountFailed").FirstOrDefault();
                                if (resultc != null)
                                    resultc.resultValue = resultc.resultValue + "," + traineeId;
                                else
                                {
                                    resultSet result1 = new resultSet();
                                    result1.resultType = "AccountFailed";
                                    result1.resultValue = traineeId.ToString();
                                    resultSet.Add(result1);
                                }
                            }
                            else
                            {
                                resultSet resultc = new resultSet();
                                resultc.resultType = "AccountFailed";
                                resultc.resultValue = traineeId.ToString();
                                resultSet.Add(resultc);
                            }
                        }
                        // end of account creation for trainee

                    }
                }

                return Json(new { ResultSet = resultSet }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ResultSet = new { status = false, message = "Invalid Input." } }, JsonRequestBehavior.AllowGet);
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        /*nice*/
        //[ActionMethodSelector]
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GenerateSyllabus(int BatchId)
        {
            if (ModelState.IsValid)
            {
                var result = batchLogic.AddSyllabus(BatchId);
                return Json(new { ResultOne = result }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ResultOne = new { status = false, message = "Invalid Input." } }, JsonRequestBehavior.AllowGet);
        }

        [PTSAuthorizeAttribute]
        public JsonResult AddTraineeToBatchClass(int traineeId, int batchClassId, string ShortName)
        {
            BatchClassLogic batchClassLogic = new BatchClassLogic();
            TraineeLogic traineeLogic = new TraineeLogic();

            var traineeBatchClass = batchClassLogic.GetTraineeBatchClass(traineeId);

            bool saved = batchClassLogic.ChangeTraineeBatchClass(traineeBatchClass.TraineeBatchClassId, batchClassId);
            string messageString = saved ? "Trainee batch class is updated successfully" : "Unable to update trainee batch class"; 

            if (!string.IsNullOrEmpty(ShortName))
            {
                traineeBatchClass.Trainee.Person.ShortName = ShortName;
                if (traineeLogic.UpdateCallSign(traineeBatchClass.TraineeId, ShortName))
                {
                    messageString += "and call sign is updated successfully";
                }

            }

            return Json(new { Result = new { status = saved, message = messageString } }, JsonRequestBehavior.AllowGet);
        }

        [PTSAuthorizeAttribute]
        public JsonResult ListTrainee()
        {
            PersonLogic personLogic = new PersonLogic();
            List<Person> result = (List<Person>)personLogic.List();
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    Id = item.PersonId,
                    Name = item.CompanyId
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        // GET: Batches
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(batchLogic.List());
        }

        // GET: Batches/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Batch batch = (Batch)batchLogic.Details((int)id);
            if (batch == null)
            {
                return HttpNotFound();
            }
            BatchClassLogic batchClassLogic = new BatchClassLogic();
            ViewBag.BatchClassList = batchClassLogic.List((int)id);
            ViewBag.TraineeList = batchLogic.ListTrainee((int)id);
            //ViewBag.BatchClassSelectList = new SelectList((List<BatchClass>)batchClassLogic.List(((BatchClass)batchClassLogic.Details((int)id)).BatchId), "BatchClassId", "BatchClassName");

            ViewBag.BatchClassSelectList = new SelectList((List<BatchClass>)batchClassLogic.List((int)id), "BatchClassId", "BatchClassName");
            return View(batch);
        }

        /*
        public PartialViewResult ShowCurriculumChange(int batchId)
        {
            BatchLogic batchLogic = new BatchLogic();
            List<CurriculumChangeView> curriculumChangeViewList = batchLogic.ShowCurriculumChange(batchId);
            return PartialView("ShowCurriculumChange", curriculumChangeViewList);
        }
        */


        [PTSAuthorizeAttribute]
        public ActionResult TraineeSyllabus(int id, string TraineeFullName, string CompanyId, string BatchClass)
        {
            ViewBag.TraineeProgramHierarchy = batchLogic.GetTraineeHierarchy(id);
            ViewBag.TraineeFullName = TraineeFullName;
            ViewBag.CompanyId = CompanyId;
            ViewBag.BatchClass = BatchClass;
            return View();
        }

        [PTSAuthorizeAttribute]
        public ActionResult DeleteBatchTrainee(int id)
        {
            if ((bool)batchLogic.DeleteBatchTrainee(id))
                return RedirectToAction("Details", new { id });
            else
            {
                TempData["Message"] = "Unable to delete";
                return RedirectToAction("Details", new { id });
            }
        }

        // GET: Batches/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            ViewBag.ProgramId = new SelectList((List<Program>)programLogic.List(), "ProgramId", "ProgramName");
            ViewBag.DayTemplateId = new SelectList((List<DayTemplate>)dayTemplateLogic.List(), "DayTemplateId", "DayTemplateName");
            ViewBag.PeriodTemplateId = new SelectList((List<PeriodTemplate>)periodTemplateLogic.List(), "PeriodTemplateId", "PeriodTemplateName");
            return View();
        }

        // POST: Batches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(Batch batch)
        {
            if (ModelState.IsValid)
            {
                if ((bool)batchLogic.Add(batch))
                    return RedirectToAction("Index");
            }

            ViewBag.ProgramId = new SelectList((List<Program>)programLogic.List(), "ProgramId", "ProgramName", batch.ProgramId);
            ViewBag.DayTemplateId = new SelectList((List<DayTemplate>)dayTemplateLogic.List(), "DayTemplateId", "DayTemplateName");
            ViewBag.PeriodTemplateId = new SelectList((List<PeriodTemplate>)periodTemplateLogic.List(), "PeriodTemplateId", "PeriodTemplateName");
            return View(batch);
        }

        // GET: Batches/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Batch batch = (Batch)batchLogic.Details((int)id);
            if (batch == null)
            {
                return HttpNotFound();
            }
            ViewBag.DayTemplateId = new SelectList((List<DayTemplate>)dayTemplateLogic.List(), "DayTemplateId", "DayTemplateName");
            ViewBag.PeriodTemplateId = new SelectList((List<PeriodTemplate>)periodTemplateLogic.List(), "PeriodTemplateId", "PeriodTemplateName");

            ViewBag.ProgramId = new SelectList((List<Program>)programLogic.List(), "ProgramId", "ProgramName", batch.ProgramId);
            return View(batch);
        }

        // POST: Batches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(Batch batch)
        {
            if (ModelState.IsValid)
            {
                if ((bool)batchLogic.Revise(batch))
                    return RedirectToAction("Index");
            }
            ViewBag.ProgramId = new SelectList((List<Program>)programLogic.List(), "ProgramId", "ProgramName", batch.ProgramId);
            ViewBag.DayTemplateId = new SelectList((List<DayTemplate>)dayTemplateLogic.List(), "DayTemplateId", "DayTemplateName");
            ViewBag.PeriodTemplateId = new SelectList((List<PeriodTemplate>)periodTemplateLogic.List(), "PeriodTemplateId", "PeriodTemplateName");
            return View(batch);
        }

        // GET: Batches/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Batch batch = (Batch)batchLogic.Details((int)id);
            if (batch == null)
            {
                return HttpNotFound();
            }
            return View(batch);
        }

        // POST: Batches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)batchLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }
    }


}
