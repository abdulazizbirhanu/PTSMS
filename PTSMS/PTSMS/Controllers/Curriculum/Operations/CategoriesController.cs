using System.Linq;
using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSBAL.Curriculum.Operations;
using System.Collections.Generic;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSBAL.Curriculum.References;
using System;
using PTSMS.Controllers.Others;

namespace PTSMS.Controllers
{
    [SessionExpire]
    public class CategoriesController : Controller
    {
        CategoryLogic categoryLogic = new CategoryLogic();
        CategoryTypeLogic categoryTypeLogic = new CategoryTypeLogic();

        List<TempCategoryName> CategoryNames = new List<TempCategoryName>();
        public JsonResult ListPhase()
        {
            List<Phase> result = (List<Phase>)categoryLogic.ListPhase();
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    Id = item.PhaseId,
                    Name = item.Name
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListStage()
        {
            List<Stage> result = (List<Stage>)categoryLogic.ListStage();
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    Id = item.StageId,
                    Name = item.Name
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }
           

        [HttpGet]
        public JsonResult ListCategory()
        {
            List<Category> result = (List<Category>)categoryLogic.List();
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    //Id = item.CategoryId,
                    Id = item.RevisionGroupId == null ? item.CategoryId : item.RevisionGroupId,
                    Type = item.CategoryName,
                    Name = item.CategoryName + "-" + item.CategoryType.Description
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListCourse(int CategoryId)
        {
            List<CourseCategory> result = (List<CourseCategory>)categoryLogic.ListCourse(CategoryId);
            return Json(new
            {
                items = result.Select(item => new
                {
                    //courseId = item.CourseId,
                    courseId = item.Course.RevisionGroupId == null ? item.Course.CourseId : item.Course.RevisionGroupId,

                    phaseId = item.PhaseId,
                    courseTitle = item.Course.CourseTitle,
                    phaseName = item.Phase.Name
                })
            });
        }


        public JsonResult AddCourse(string CourseId, int CategoryId)
        {
            if (!(String.IsNullOrEmpty(CourseId) || String.IsNullOrWhiteSpace(CourseId)))
            {
                string[] CourseIdArray = CourseId.Split(',');
                object result = categoryLogic.AddCourse(CourseIdArray.ToList(), CategoryId);
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = "Invalid input." }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveCourse(int courseCategoryId)
        {
            object result = categoryLogic.RemoveCourse(courseCategoryId);
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListLesson(int CategoryId)
        {
            List<LessonCategory> result = (List<LessonCategory>)categoryLogic.ListLesson(CategoryId);
            return Json(new
            {
                items = result.Select(item => new
                {
                    lessonId = item.LessonId,
                    phaseId = item.PhaseId,
                    lessonName = item.Lesson.LessonName,
                    phaseName = item.Phase.Name
                })
            });
        }

        public JsonResult AddLesson(string LessonId, int CategoryId, int phaseId, int stageId)
        {

            if (!(String.IsNullOrEmpty(LessonId) || String.IsNullOrWhiteSpace(LessonId)))
            {
                string[] LessonIdArray = LessonId.Split(',');
                object result = categoryLogic.AddLesson(LessonIdArray.ToList(), CategoryId, phaseId, stageId);
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = "Invalid input." }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveLessonCategory(int lessonCategoryId)
        {
            object result = categoryLogic.RemoveLessonCategory(lessonCategoryId);
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult CategoryDetailPartialView(int ProgramCategoryId)
        {
            Category category = categoryLogic.CategoryDetailPartialView(ProgramCategoryId);
            return PartialView("CategoryDetailPartialView", category);
        }

        // GET: Categories
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            var a = categoryLogic.List();
            return View(a);
        }

        // GET: Categories/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = (Category)categoryLogic.Details((int)id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create    
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            Construct();
            ViewBag.CategoryNamee = new SelectList(CategoryNames, "Value", "Text");
            ViewBag.CategoryTypeId = new SelectList(((List<CategoryType>)categoryTypeLogic.List()).Select(item => new
            {
                CategoryTypeId = item.CategoryTypeId,
                Type = item.Type + "" + (item.IsTypeRating ? "-(TypeRating)": "")
            }).ToList(), "CategoryTypeId", "Type");
            ViewBag.RevisionGroupId = new SelectList((List<Category>)categoryLogic.List(), "CategoryId", "Status");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(Category category)
        {
            //if (ModelState.IsValid)
            //{
            if ((bool)categoryLogic.Add(category))
                return RedirectToAction("Index");
            //}
            Construct();
            ViewBag.CategoryNamee = new SelectList(CategoryNames, "Value", "Text");
            ViewBag.CategoryTypeId = new SelectList(((List<CategoryType>)categoryTypeLogic.List()).Select(item => new
            {
                CategoryTypeId = item.CategoryTypeId,
                Type = item.Type + "" + (item.IsTypeRating ? "-(TypeRating)" : "")
            }).ToList(), "CategoryTypeId", "Type");
            ViewBag.RevisionGroupId = new SelectList((List<Category>)categoryLogic.List(), "CategoryId", "Status", category.RevisionGroupId);
            return View(category);
        }

        // GET: Categories/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = (Category)categoryLogic.Details((int)id);
            if (category == null)
            {
                return HttpNotFound();
            }
            Construct();
            ViewBag.CategoryNamee = new SelectList(CategoryNames, "Value", "Text");
            ViewBag.CategoryTypeId = new SelectList(((List<CategoryType>)categoryTypeLogic.List()).Select(item => new
            {
                CategoryTypeId = item.CategoryTypeId,
                Type = item.Type + "" + (item.IsTypeRating ? "-(TypeRating)" : "")
            }).ToList(), "CategoryTypeId", "Type");

            ViewBag.RevisionGroupId = new SelectList((List<Category>)categoryLogic.List(), "CategoryId", "Status", category.RevisionGroupId);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(Category category)
        {
            //if (ModelState.IsValid)
            // {
            if ((bool)categoryLogic.Revise(category))
                return RedirectToAction("Index");
            //}
            Construct();
            ViewBag.CategoryNamee = new SelectList(CategoryNames);
            ViewBag.CategoryTypeId = new SelectList((List<CategoryType>)categoryTypeLogic.List(), "CategoryTypeId", "Type", category.CategoryTypeId);
            ViewBag.RevisionGroupId = new SelectList((List<Category>)categoryLogic.List(), "CategoryId", "Status", category.RevisionGroupId);
            return View(category);
        }

        // GET: Categories/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = (Category)categoryLogic.Details((int)id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5      
        [PTSAuthorizeAttribute]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)categoryLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }

        public void Construct()
        {
            CategoryNames.Add(new TempCategoryName
            {
                Text = "Flying",
                Value = "Flying"
            });
            CategoryNames.Add(new TempCategoryName
            {
                Text = "FTD",
                Value = "FTD"
            });
            CategoryNames.Add(new TempCategoryName
            {
                Text = "Ground",
                Value = "Ground"
            });
        }
    }
    public class TempCategoryName
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
