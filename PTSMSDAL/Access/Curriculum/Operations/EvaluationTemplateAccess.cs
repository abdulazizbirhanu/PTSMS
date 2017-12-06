using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Curriculum.View;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.Operations
{
    public class EvaluationTemplateAccess
    {
        private PTSContext db = new PTSContext();

        public List<EvaluationTemplate> List()
        {
            return db.EvaluationTemplates.Include(c => c.PreviousEvaluationTemplate).Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public object Details(int evaluationTemplateId)
        {
            try
            {
                EvaluationTemplateView evaluationTemplateView = new EvaluationTemplateView();

                var evaluationTemplates = db.EvaluationTemplates.ToList();

                var evaluationTemplate = evaluationTemplates.Single(pr => pr.EvaluationTemplateId == evaluationTemplateId && pr.EndDate > DateTime.Now);
                if (evaluationTemplate != null)
                {
                    evaluationTemplateView.EvaluationTemplate = (EvaluationTemplate)evaluationTemplate;

                    var EvaluationCategories = db.EvaluationCategories.Where(evaCat => evaCat.EvaluationTemplateId.Equals(evaluationTemplateView.EvaluationTemplate.EvaluationTemplateId)).ToList();

                    List<EvaluationCategoryView> evaluationCategoryViewList = new List<EvaluationCategoryView>();
                    EvaluationCategoryView evaluationCategoryView = null;

                    foreach (var evaCat in EvaluationCategories)
                    {
                        evaluationCategoryView = new EvaluationCategoryView();
                        var evaluationItems = db.EvaluationItems.Where(evaItem => evaItem.EvaluationCategoryId.Equals(evaCat.EvaluationCategoryId)).ToList();
                        List<EvaluationItem> EvaluationItemList = new List<EvaluationItem>();

                        foreach (var evaItem in evaluationItems)
                        {
                            EvaluationItemList.Add(evaItem);
                        }
                        evaluationCategoryView.EvaluationCategory = evaCat;
                        if (EvaluationItemList.Count() > 0)
                            evaluationCategoryView.EvaluationItems.AddRange(EvaluationItemList);
                        evaluationCategoryViewList.Add(evaluationCategoryView);
                    }
                    if (evaluationCategoryViewList.Count() > 0)
                    {
                        evaluationTemplateView.EvaluationCategories.AddRange(evaluationCategoryViewList);
                    }
                    return evaluationTemplateView;
                }
                return new EvaluationTemplateView();// Success EvaluationTemplateView
            }
            catch (System.Exception e)
            {
                return new EvaluationTemplateView(); ; // Exception
            }
        }

        public LessonEvaluationTemplate GetLessonEvaluationTemplate(int lessonId)
        {
            try
            {
                PTSContext db = new PTSContext();
                return db.LessonEvaluationTemplates.Where(LET => LET.LessonCategory.LessonId == lessonId).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                return new LessonEvaluationTemplate();
            }
        }

        public EvaluationTemplateView EvaluationTemplateDetail(int lessonEvaluationTemplateId)
        {
            try
            {
                LessonEvaluationTemplate lessonEvaluationTemplate = db.LessonEvaluationTemplates.Find(lessonEvaluationTemplateId);

                if (lessonEvaluationTemplate != null)
                {
                    var evaluationTemplateList = db.EvaluationTemplates.Where(c => ((c.RevisionGroupId == null && c.EvaluationTemplateId == lessonEvaluationTemplate.EvaluationTemplateId)
                        || (c.RevisionGroupId != null && c.RevisionGroupId == lessonEvaluationTemplate.EvaluationTemplateId)) && c.Status == "Active").ToList();

                    if (evaluationTemplateList.Count > 0)
                    {
                        EvaluationTemplateView evaluationTemplateView = new EvaluationTemplateView();

                        var evaluationTemplate = evaluationTemplateList.FirstOrDefault();

                        if (evaluationTemplate != null)
                        {
                            evaluationTemplateView.EvaluationTemplate = evaluationTemplate;

                            var EvaluationCategories = db.EvaluationCategories.Where(evaCat => evaCat.EvaluationTemplateId.Equals(evaluationTemplateView.EvaluationTemplate.EvaluationTemplateId)).ToList();

                            List<EvaluationCategoryView> evaluationCategoryViewList = new List<EvaluationCategoryView>();
                            EvaluationCategoryView evaluationCategoryView = null;

                            foreach (var evaCat in EvaluationCategories)
                            {
                                evaluationCategoryView = new EvaluationCategoryView();
                                var evaluationItems = db.EvaluationItems.Where(evaItem => evaItem.EvaluationCategoryId.Equals(evaCat.EvaluationCategoryId)).ToList();
                                List<EvaluationItem> EvaluationItemList = new List<EvaluationItem>();

                                foreach (var evaItem in evaluationItems)
                                {
                                    EvaluationItemList.Add(evaItem);
                                }
                                evaluationCategoryView.EvaluationCategory = evaCat;
                                if (EvaluationItemList.Count() > 0)
                                    evaluationCategoryView.EvaluationItems.AddRange(EvaluationItemList);
                                evaluationCategoryViewList.Add(evaluationCategoryView);
                            }
                            if (evaluationCategoryViewList.Count() > 0)
                            {
                                evaluationTemplateView.EvaluationCategories.AddRange(evaluationCategoryViewList);
                            }
                            return evaluationTemplateView;
                        }
                    }
                }
                return new EvaluationTemplateView();// Success EvaluationTemplateView
            }
            catch (System.Exception e)
            {
                return new EvaluationTemplateView(); ; // Exception
            }
        }


        public bool Add(EvaluationTemplate evaluationTemplate)
        {
            try
            {
                evaluationTemplate.StartDate = DateTime.Now;
                evaluationTemplate.EndDate = Constants.EndDate;
                evaluationTemplate.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                evaluationTemplate.CreationDate = DateTime.Now;

                db.EvaluationTemplates.Add(evaluationTemplate);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(EvaluationTemplate evaluationTemplate)
        {
            try
            {
                evaluationTemplate.RevisionDate = DateTime.Now;
                evaluationTemplate.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(evaluationTemplate).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Delete(int id)
        {
            try
            {
                EvaluationTemplate evaluationTemplate = db.EvaluationTemplates.Find(id);
                evaluationTemplate.EndDate = DateTime.Now;
                evaluationTemplate.EvaluationTemplateName += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(evaluationTemplate).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
    }
}