using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Operations;
using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.IO;
using PTSMSDAL.Context;
using System.Web.Mvc;
using PTSMSDAL.Access.Curriculum.Relations;
using PTSMSDAL.Access.Utility;

namespace PTSMSBAL.Curriculum.Operations
{
    public class ModuleLogic
    {
        ModuleAccess moduleAccess = new ModuleAccess();

        public object ListExam(int moduleId)
        {
            return moduleAccess.ListExam(moduleId);
        }

        public object AddExam(List<string> examIdList, int moduleId)
        {
            return moduleAccess.AddExam(examIdList, moduleId);
        }

        public object RemoveModuleExam(int moduleExamId)
        {
            return moduleAccess.RemoveModuleExam(moduleExamId);
        }

        public object AddGroundLesson(List<string> moduleGroundLessonIdList, int courseModuleId)
        {
            return moduleAccess.AddGroundLesson(moduleGroundLessonIdList, courseModuleId);
        }

        public object RemoveModuleGroundLesson(int moduleGroundLessonId)
        {
            return moduleAccess.RemoveModuleGroundLesson(moduleGroundLessonId);
        }

        public object List()
        {
            return moduleAccess.List();
        }

        public List<Module> ListModule()
        {
            List<Module> ModuleList = (List<Module>)moduleAccess.List();
            PTSContext db = new PTSContext();

            foreach (var module in ModuleList)
            {
                var moduleReferences = db.ModuleReferences.Where(mr => mr.ModuleId == module.ModuleId).ToList();

                List<SelectListItem> fileNameList = new List<SelectListItem>();

                foreach (var reference in moduleReferences)
                {
                    fileNameList.Add(new SelectListItem { Text = reference.FileName, Value = reference.ReferenceURL });
                }
                module.DropDownFileLists = fileNameList;
            }
            return ModuleList;
        }

        public object List(int CourseCategoryId)
        {
            return moduleAccess.List(CourseCategoryId);
        }

        public object Details(int id)
        {
            Module module = (Module)moduleAccess.Details(id);
            PTSContext db = new PTSContext();
           // item.RevisionGroupId == null ? item.ModuleId : item.RevisionGroupId
            var moduleReferences = db.ModuleReferences.Where(mr => mr.ModuleId == module.ModuleId).ToList();


            List<SelectListItem> fileNameList = new List<SelectListItem>();

            foreach (var reference in moduleReferences)
            {
                fileNameList.Add(new SelectListItem { Text = reference.FileName, Value = reference.Module.ModuleId.ToString() });
            }
            module.DropDownFileLists = fileNameList;

            return module;
        }
        public Module ModuleDetail(int courseModuleId)
        {
            Module module = (Module)moduleAccess.ModuleDetail(courseModuleId);
            PTSContext db = new PTSContext();

            var moduleReferences = db.ModuleReferences.Where(mr => mr.ModuleId == module.ModuleId).ToList();

            List<SelectListItem> fileNameList = new List<SelectListItem>();

            foreach (var reference in moduleReferences)
            {
                fileNameList.Add(new SelectListItem { Text = reference.FileName, Value = reference.Module.ModuleId.ToString() });
            }
            module.DropDownFileLists = fileNameList;
            return module;
        }



        public object Add(Module module, IEnumerable<HttpPostedFileBase> ModuleReferenceFiles)
        {
            module.Status = "Active";
            module.RevisionNo = 1;
            return moduleAccess.Add(module, ModuleReferenceFiles, "");
        }




        public object Revise(IEnumerable<HttpPostedFileBase> ModuleReferenceFiles, Module module)
        {
            Module mod = (Module)moduleAccess.Details(module.ModuleId);

            string moduleCode = mod.ModuleCode;

            mod.Status = "Replaced";
            mod.ModuleCode += "_" + DateTime.Now.ToString("ddMMyyyyHHmmss");
            module.RevisionNo = mod.RevisionNo + 1;
            module.Status = "Active";

            if (mod.RevisionGroupId == null)
                module.RevisionGroupId = module.ModuleId;
            else
                module.RevisionGroupId = mod.RevisionGroupId;

            moduleAccess.Revise(mod);
            if (moduleAccess.Add(module, ModuleReferenceFiles, moduleCode))
            {
                //Copy old module activities into the revised module
                UtilityClass utilityClass = new UtilityClass();
                ModuleActivityAccess moduleActivityAccess = new ModuleActivityAccess();
                int moduleId = utilityClass.GetLatestIdNumber("MODULE");

                List<ModuleActivity> moduleActivityList = moduleActivityAccess.GetModuleActivityList(mod.ModuleId);
                foreach (var moduleActivity in moduleActivityList)
                {
                    moduleActivityAccess.Add(new ModuleActivity
                    {
                        ModuleId = moduleId,
                        ModuleActivityName = moduleActivity.ModuleActivityName,
                        EstimatedDuration = moduleActivity.EstimatedDuration,
                        CreationDate = DateTime.Now,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.MaxValue,
                        RevisionDate = DateTime.Now
                    });
                }
                return true;
            }
            return false;
        }

        public object Delete(int id)
        {
            return moduleAccess.Delete(id);
        }

        /*Nice */
        public object FilteredListModule(int batchClassId)
        {
            return moduleAccess.FilteredListModule(batchClassId);
            //throw new NotImplementedException();
        }
        public object FilteredModuleExam(int eCourseId)
        {
            return moduleAccess.FilteredModuleExam(eCourseId);
        }

        public void FilesView(string moduleCode, string FileName, ref string filePath)
        {
            var targetPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/ModuleReference/");
            string targetPath2 = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/ModuleReference/" + moduleCode + "/");

            if (System.IO.Directory.Exists(targetPath2))
            {
                filePath = targetPath2 + FileName;
            }
            else
            {
                filePath = targetPath + FileName;
            }
        }
        /*Nice */
    }
}