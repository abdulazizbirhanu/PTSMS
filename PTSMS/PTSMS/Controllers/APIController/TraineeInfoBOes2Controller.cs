using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using PTSMSDAL.APIModels.Models;
using PTSMSDAL.Context;

namespace PTSMS.Controllers.APIController
{
    public class TraineeInfoBOes2Controller : Controller
    {
        private EAA_API_Context db2 = new EAA_API_Context();
        private PROD_CMSEntities db = new PROD_CMSEntities();

        // GET: TraineeInfoBOes2
        public ActionResult Index()
        {
            DateTime oneweekback = DateTime.Now.AddDays(-7);
            return View(db2.TraineeInfobo.Where(r => r.registrationDate >= oneweekback).ToList());
        }

        // GET: TraineeInfoBOes2/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            TrainingApplication traineeInfoBO = db.TrainingApplications.Find(id);
            if (traineeInfoBO == null)
            {
                return HttpNotFound();
            }
            return View(traineeInfoBO);
        }

        // GET: TraineeInfoBOes2/Create
        public ActionResult Create()
        {
            return View();
        }
        public void Export()
        {
            DateTime oneweekback = DateTime.Now.AddDays(-15);
            List<TraineeInfoBO> traineeBO = db2.TraineeInfobo.Where(r=>r.registrationDate >= oneweekback).ToList();
            List<TraineeInfoBOWithoutID> ListTraineeInfo = new List<TraineeInfoBOWithoutID>();
            foreach (TraineeInfoBO trainee in traineeBO)
            {
                TraineeInfoBOWithoutID temp = new TraineeInfoBOWithoutID();
                temp.ApplyingForProgram = trainee.ApplyingForProgram;
                temp.Category = trainee.Category;
                temp.CellPhone = trainee.CellPhone;
                temp.CertificateType = trainee.CertificateType;
                temp.City = trainee.City;
                temp.Country = trainee.Country;
                temp.EducationalLevel = trainee.EducationalLevel;
                temp.Email = trainee.Email;
                temp.FirstName = trainee.FirstName;
                temp.Gender = trainee.Gender;
                temp.HomePhone = trainee.HomePhone;
                temp.LastName = trainee.LastName;
                temp.MiddleName = trainee.MiddleName;
                temp.registrationDate = trainee.registrationDate.Date;
                temp.Salutation = trainee.Salutation;
                ListTraineeInfo.Add(temp);
            }
            var traineeBOSelected = new List<TraineeInfoBO>();
            GridView gridview = new GridView();
            gridview.DataSource = ListTraineeInfo;
            gridview.DataBind();
            Response.ClearContent();
            Response.Buffer = true;

            Response.AddHeader("content-disposition", "attachment;filename = " + DateTime.Now.ToString("dd -MMM-yyyy - hh - mm") + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    gridview.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
            //Microsoft.Office.Interop.Excel.Application excel;
            //Microsoft.Office.Interop.Excel.Workbook worKbooK;
            //excel = new Microsoft.Office.Interop.Excel.Application();
            //excel.Visible = false;
            //excel.DisplayAlerts = false;
            //worKbooK = excel.Workbooks.Add(Type.Missing);
            //List<TraineeInfoBO> traineeBODates = new List<TraineeInfoBO>();
            //traineeBODates = traineeBO.DistinctBy(t => t.registrationDate).ToList();
            //foreach (TraineeInfoBO traineeDate in traineeBODates)
            //{

            //    Microsoft.Office.Interop.Excel.Worksheet worksheet;
            //    worksheet = (Microsoft.Office.Interop.Excel.Worksheet)worKbooK.ActiveSheet;
            //    List<TraineeInfoBO> traineeBOselected = traineeBO.Where(t => t.registrationDate == traineeDate.registrationDate).ToList();
            //    worksheet.Name = traineeDate.registrationDate.ToString("dd - MMM - yyyy");
            //    int rowcount = 0;
            //    foreach (TraineeInfoBO trInfo in traineeBOselected)
            //    {
            //        rowcount += 1;
            //        if (rowcount == 1)
            //        {
            //            worksheet.Rows[rowcount].Font.Bold = true;
            //            worksheet.Rows[rowcount].Font.Color = System.Drawing.ColorTranslator.FromHtml("#FF0000");
            //            worksheet.Cells[rowcount, 1] = "Salutation";
            //            worksheet.Cells[rowcount, 2] = "First Name";
            //            worksheet.Cells[rowcount, 3] = "Middle Name";
            //            worksheet.Cells[rowcount, 4] = "Last Name";
            //            worksheet.Cells[rowcount, 5] = "Gender";
            //            worksheet.Cells[rowcount, 6] = "Email";
            //            worksheet.Cells[rowcount, 7] = "Cell Phone";
            //            worksheet.Cells[rowcount, 8] = "Home Phone";
            //            worksheet.Cells[rowcount, 9] = "City";
            //            worksheet.Cells[rowcount, 10] = "Country";
            //            worksheet.Cells[rowcount, 11] = "Educational Level";
            //            worksheet.Cells[rowcount, 12] = "Applying For Program";
            //            worksheet.Cells[rowcount, 13] = "Certificate Type";
            //            worksheet.Cells[rowcount, 14] = "Category";
            //        }
            //        if (rowcount > 1)
            //        {
            //            worksheet.Rows[rowcount].Font.Bold = false;
            //            worksheet.Rows[rowcount].Font.Color = System.Drawing.Color.Black;
            //            worksheet.Cells[rowcount, 1] = trInfo.Salutation;
            //            worksheet.Cells[rowcount, 2] = trInfo.FirstName;
            //            worksheet.Cells[rowcount, 3] = trInfo.MiddleName;
            //            worksheet.Cells[rowcount, 4] = trInfo.LastName;
            //            worksheet.Cells[rowcount, 5] = trInfo.Gender;
            //            worksheet.Cells[rowcount, 6] = trInfo.Email;
            //            worksheet.Cells[rowcount, 7] = trInfo.CellPhone;
            //            worksheet.Cells[rowcount, 8] = trInfo.HomePhone;
            //            worksheet.Cells[rowcount, 9] = trInfo.City;
            //            worksheet.Cells[rowcount, 10] = trInfo.Country;
            //            worksheet.Cells[rowcount, 11] = trInfo.EducationalLevel;
            //            worksheet.Cells[rowcount, 12] = trInfo.ApplyingForProgram;
            //            worksheet.Cells[rowcount, 13] = trInfo.CertificateType;
            //            worksheet.Cells[rowcount, 14] = trInfo.Category;
            //        }
            //    }

            //}

            //string targateAddress = Server.MapPath("~/ExcelFiles");
            //if (!Directory.Exists(targateAddress))
            //    Directory.CreateDirectory(targateAddress);
            //string fileName = "applicantData" + DateTime.Now.ToString("dd MMM yyy") + ".xlsx";
            ////File.Create(targateAddress);

            //worKbooK.SaveAs(targateAddress + "/" + fileName);
            //worKbooK.Close();
            //excel.Quit();



            //FileInfo file = new FileInfo(targateAddress + "/" + fileName);
            //if (file.Exists)
            //{
            //    Response.Clear();
            //    Response.ClearHeaders();
            //    Response.ClearContent();
            //    Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            //    Response.AddHeader("Content-Type", "application/Excel");
            //    Response.ContentType = "application/vnd.xls";
            //    Response.AddHeader("Content-Length", file.Length.ToString());
            //    Response.WriteFile(file.FullName);
            //    Response.End();
            //}


        }
        // POST: TraineeInfoBOes2/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Salutation,FirstName,MiddleName,LastName,Gender,Email,CellPhone,HomePhone,City,Country,EducationalLevel,ApplyingForProgram,CertificateType,Category,registrationDate")] TrainingApplication traineeInfoBO)
        {
            if (ModelState.IsValid)
            {
                db.TrainingApplications.Add(traineeInfoBO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(traineeInfoBO);
        }

        // GET: TraineeInfoBOes2/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TrainingApplication traineeInfoBO = db.TrainingApplications.Find(id);
        //    if (traineeInfoBO == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(traineeInfoBO);
        //}

        //// POST: TraineeInfoBOes2/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id,Salutation,FirstName,MiddleName,LastName,Gender,Email,CellPhone,HomePhone,City,Country,EducationalLevel,ApplyingForProgram,CertificateType,Category")] TraineeInfoBO traineeInfoBO)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(traineeInfoBO).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(traineeInfoBO);
        //}

        //// GET: TraineeInfoBOes2/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TraineeInfoBO traineeInfoBO = db.TraineeInfobo.Find(id);
        //    if (traineeInfoBO == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(traineeInfoBO);
        //}

        //// POST: TraineeInfoBOes2/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    TraineeInfoBO traineeInfoBO = db.TraineeInfobo.Find(id);
        //    db.TraineeInfobo.Remove(traineeInfoBO);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
