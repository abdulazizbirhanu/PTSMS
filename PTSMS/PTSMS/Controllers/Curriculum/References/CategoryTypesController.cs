using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSBAL.Curriculum.References;
using System.Collections.Generic;
using System.Linq;

namespace PTSMS.Controllers
{
   // [PTSAuthorizeAttribute]
    public class CategoryTypesController : Controller
    {
        CategoryTypeLogic categoryTypeLogic = new CategoryTypeLogic();

        [HttpPost]
        public JsonResult GetTypeRateCategoryTypes(bool isTypeRating)
        {
            List<CategoryType> result = (List<CategoryType>)categoryTypeLogic.List();
            //var TypeRatingCategoryType = result.Where(CT=>CT.IsTypeRating).ToList()
            return Json(new
            {
                resultData = result.Where(CT => CT.IsTypeRating == isTypeRating && !CT.Type.ToLower().Contains("ground")).ToList().Select(item => new
                {
                    Id = item.CategoryTypeId,
                    Name = item.Description
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        // GET: CategoryTypes
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(categoryTypeLogic.List());
        }

        // GET: CategoryTypes/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryType categoryType = (CategoryType)categoryTypeLogic.Details((int)id);
            if (categoryType == null)
            {
                return HttpNotFound();
            }
            return View(categoryType);
        }

        // GET: CategoryTypes/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList((List<CategoryType>)categoryTypeLogic.List(), "CategoryTypeId", "Type");
            return View();
        }

        // POST: CategoryTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(CategoryType categoryType)
        {
            if (ModelState.IsValid)
            {
                if ((bool)categoryTypeLogic.Add(categoryType))
                    return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList((List<CategoryType>)categoryTypeLogic.List(), "CategoryTypeId", "Type", categoryType.RevisionGroupId);
            return View(categoryType);
        }

        // GET: CategoryTypes/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryType categoryType = (CategoryType)categoryTypeLogic.Details((int)id);
            if (categoryType == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList((List<CategoryType>)categoryTypeLogic.List(), "CategoryTypeId", "Type", categoryType.RevisionGroupId);
            return View(categoryType);
        }

        // POST: CategoryTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(CategoryType categoryType)
        {
            if (ModelState.IsValid)
            {
                if ((bool)categoryTypeLogic.Revise(categoryType))
                    return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList((List<CategoryType>)categoryTypeLogic.List(), "CategoryTypeId", "Type", categoryType.RevisionGroupId);
            return View(categoryType);
        }

        // GET: CategoryTypes/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryType categoryType = (CategoryType)categoryTypeLogic.Details((int)id);
            if (categoryType == null)
            {
                return HttpNotFound();
            }
            return View(categoryType);
        }

        // POST: CategoryTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)categoryTypeLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }        
    }
}
