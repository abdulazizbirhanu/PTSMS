using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Scheduling.View;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Curriculum.Relations;

namespace PTSMSDAL.Access.Scheduling.Relations
{
    public class TraineeEvaluationTemplateAccess
    {
        PTSContext dbContext = new PTSContext();
        public List<TraineeEvaluationTemplateView> EvaluationTemplateList(int traineeId, int lessonId, int sequence)
        {
            //select Equipment it's certificate is not expired
            try
            {
                PTSContext db = new PTSContext();
                int rightSideTablesRows = 0;
                int LeftSideTablesRows = 0;
                TraineeEvaluationTemplateView TraineeEvaluationTemplateRightView = new TraineeEvaluationTemplateView();
                TraineeEvaluationTemplateView TraineeEvaluationTemplateLeftView = new TraineeEvaluationTemplateView();
                List<TraineeEvaluationTemplateView> evaluationTemplateView = new List<TraineeEvaluationTemplateView>();
                DateTime today = DateTime.Now;

                var traineeLessonResult = db.TraineeLessons
                    .Where(t => t.TraineeId == traineeId && t.LessonId == lessonId && t.Sequence == t.Sequence).ToList();

                //var resultGoup = result.GroupBy(etc => etc.TET.TraineeEvaluationTemplateId).Select(grp => grp.FirstOrDefault()).ToList();
                if (traineeLessonResult.Count() > 0)
                {
                    var traineeLesson = traineeLessonResult.FirstOrDefault();

                    TraineeEvaluationTemplateLeftView.Id = traineeLesson.EvaluationTemplateId;
                    TraineeEvaluationTemplateLeftView.Name = traineeLesson.EvaluationTemplate.EvaluationTemplateName;
                    TraineeEvaluationTemplateLeftView.Remark = traineeLesson.Remark;
                    TraineeEvaluationTemplateLeftView.IsEvaluated = false;

                    TraineeEvaluationTemplateRightView.Id = traineeLesson.EvaluationTemplateId;
                    TraineeEvaluationTemplateRightView.Name = traineeLesson.EvaluationTemplate.EvaluationTemplateName;
                    TraineeEvaluationTemplateRightView.Remark = traineeLesson.Remark;
                    TraineeEvaluationTemplateRightView.IsEvaluated = false;
                    if (traineeLesson.OverallGradeId != null)
                    {
                        TraineeEvaluationTemplateLeftView.OverallGradeId = traineeLesson.OverallGradeId;
                        TraineeEvaluationTemplateRightView.OverallGradeId = traineeLesson.OverallGradeId;

                        TraineeEvaluationTemplateLeftView.IsEvaluated = true;
                        TraineeEvaluationTemplateRightView.IsEvaluated = true;
                    }
                    var lessonScores = db.LessonScores.Where(LS => LS.EndDate > DateTime.Now).ToList();

                    List<LessonScoreView> LessonScoreList = new List<LessonScoreView>();
                    foreach (var item in lessonScores)
                    {
                        LessonScoreList.Add(new LessonScoreView
                        {
                            Id = item.LessonScoreId,
                            Name = item.ScoreLetter,
                            Score = item.ScoreLetterValue
                        });
                    }

                    if (LessonScoreList.Count > 0)
                    {
                        TraineeEvaluationTemplateLeftView.LessonScores.AddRange(LessonScoreList);
                        TraineeEvaluationTemplateRightView.LessonScores.AddRange(LessonScoreList);
                    }


                    var traineeEvaluationCategories = db.TraineeEvaluationCategories.Where(EC => EC.TraineeLessonId == traineeLesson.TraineeLessonId 
                    && EC.TraineeLesson.TraineeId == traineeId 
                    && EC.TraineeLesson.LessonId == lessonId).ToList();

                    traineeEvaluationCategories = traineeEvaluationCategories.OrderBy(e => e.sequenceNo).ToList();
                    List<EvaluationCategoriesView> EvaluationCategories = new List<EvaluationCategoriesView>();
                    EvaluationCategoriesView evaluationCategoryView = null;
                    foreach (var evaCategory in traineeEvaluationCategories)
                    {
                        evaluationCategoryView = new EvaluationCategoriesView();
                        evaluationCategoryView.Id = evaCategory.TraineeEvaluationCategoryId;
                        evaluationCategoryView.Name = evaCategory.EvaluationCategoryName;
                        evaluationCategoryView.sequenceNo = evaCategory.sequenceNo;

                        var evaluationItemList = db.TraineeEvaluationItems.Where(EI => EI.TraineeEvaluationCategoryId == evaCategory.TraineeEvaluationCategoryId 
                        && EI.TraineeEvaluationCategory.TraineeLesson.TraineeId == traineeId 
                        && EI.TraineeEvaluationCategory.TraineeLesson.LessonId == lessonId).ToList();

                        evaluationItemList = evaluationItemList.OrderBy(e => e.sequenceNo).ToList();

                        List<EvaluationItemsView> EvaluationItems = new List<EvaluationItemsView>();

                        foreach (var item in evaluationItemList)
                        {
                            if (item.LessonScoreId != null)
                            {
                                EvaluationItems.Add(new EvaluationItemsView
                                {
                                    Id = item.TraineeEvaluationItemId,
                                    Name = item.EvaluationItemName,
                                    ScoreLevelId = (int)item.LessonScoreId
                                });
                            }
                            else
                            {
                                EvaluationItems.Add(new EvaluationItemsView
                                {
                                    Id = item.TraineeEvaluationItemId,
                                    Name = item.EvaluationItemName,
                                    ScoreLevelId = 0
                                });
                            }
                        }

                        if (EvaluationItems.Count > 0)
                        {
                            evaluationCategoryView.EvaluationItem.AddRange(EvaluationItems);
                            EvaluationCategories.Add(evaluationCategoryView);
                        }
                    }
                    if (EvaluationCategories.Count > 0)
                    {
                        
                        foreach (EvaluationCategoriesView ev in EvaluationCategories)
                        {
                            if (ev.sequenceNo > 0)
                            {
                                if (ev.sequenceNo <= EvaluationCategories.Count / 2)
                                {
                                    TraineeEvaluationTemplateLeftView.EvaluationCategory.Add(ev);
                                    LeftSideTablesRows += ev.EvaluationItem.Count;
                                }
                                else
                                {
                                    TraineeEvaluationTemplateRightView.EvaluationCategory.Add(ev);
                                    rightSideTablesRows += ev.EvaluationItem.Count;
                                }
                            }
                            else
                            {

                                if (LeftSideTablesRows < rightSideTablesRows + ev.EvaluationItem.Count)
                                {
                                    TraineeEvaluationTemplateLeftView.EvaluationCategory.Add(ev);
                                    LeftSideTablesRows += ev.EvaluationItem.Count;
                                }
                                else
                                {
                                    TraineeEvaluationTemplateRightView.EvaluationCategory.Add(ev);
                                    rightSideTablesRows += ev.EvaluationItem.Count;
                                }
                            }
                           
                        }
                    }
                }
                evaluationTemplateView.Add(TraineeEvaluationTemplateLeftView);
                evaluationTemplateView.Add(TraineeEvaluationTemplateRightView);

                return evaluationTemplateView;
            }
            catch (Exception ex)
            {
                return new List<TraineeEvaluationTemplateView>();
            }
        }

        public bool IsTraineeLessonHasEvaluationItem(int traineeId, int lessonId, int sequence)
        {
            try
            {
                PTSContext db = new PTSContext();
                var result = db.TraineeEvaluationCategories
                      .Where(tev => tev.TraineeLesson.LessonId == lessonId
                      && tev.TraineeLesson.TraineeId == traineeId
                      && tev.TraineeLesson.Sequence == sequence).ToList();
                return result.Count > 0 ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IsAlreadyEvaluated(int traineeId, int lessonId, int sequence)
        {
            try
            {
                PTSContext db = new PTSContext();
                var result = db.TraineeEvaluationCategories
                      .Where(tev => tev.TraineeLesson.LessonId == lessonId
                      && tev.TraineeLesson.TraineeId == traineeId
                      && tev.TraineeLesson.Sequence == sequence && tev.TraineeLesson.Status == "Agreed").ToList();
                return result.Count > 0 ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public OperationResult SaveTraineeEvaluationTemplateItems(string[] evaluationTemplateItems, int overAllGradeId, int traineeId, int lessonId, string remark, string TimeIn, string TimeOut, string FlightTime, string FlightDate)
        {
            try
            {
                string message = "";
                PTSContext db = new PTSContext();

                foreach (var item in evaluationTemplateItems)
                {
                    if (!(String.IsNullOrEmpty(item) || String.IsNullOrWhiteSpace(item)))
                    {
                        string[] ItemDetailArray = item.Split('-');
                        if (!(String.IsNullOrEmpty(ItemDetailArray[0]) || String.IsNullOrWhiteSpace(ItemDetailArray[0]) || String.IsNullOrEmpty(ItemDetailArray[1]) || String.IsNullOrWhiteSpace(ItemDetailArray[1])))
                        {
                            int evaluationTemplateId = Convert.ToInt32(ItemDetailArray[0]);
                            int letterScoreId = Convert.ToInt32(ItemDetailArray[1]);

                            //Save trainee evaluation
                            var traineeEvaluationItemsResult = db.TraineeEvaluationItems.Where(EI => EI.TraineeEvaluationItemId == evaluationTemplateId).ToList();
                            if (traineeEvaluationItemsResult.Count() > 0)
                            {
                                TraineeEvaluationItem traineeEvaluationItem = traineeEvaluationItemsResult.FirstOrDefault();
                                traineeEvaluationItem.LessonScoreId = letterScoreId;

                                db.Entry(traineeEvaluationItem).State = EntityState.Modified;
                                if (db.SaveChanges() <= 0)
                                    message = message + traineeEvaluationItemsResult.FirstOrDefault().EvaluationItemName + " is not saved. ";
                            }
                        }
                    }
                }

                //Save Trainee Lesson Info
                var traineeLessonResult = db.TraineeLessons.Where(TL => TL.LessonId == lessonId && TL.TraineeId == traineeId).ToList();
                if (traineeLessonResult.Count() > 0)
                {
                    TraineeLesson traineeLesson = traineeLessonResult.FirstOrDefault();
                    traineeLesson.Remark = remark;
                    traineeLesson.OverallGradeId = overAllGradeId;
                    traineeLesson.Status = "Evaluated";
                    traineeLesson.TimeIN = TimeIn;
                    traineeLesson.TimeOut = TimeOut;
                    traineeLesson.FlightTime = FlightTime;
                    traineeLesson.FlightDate = FlightDate;
                    traineeLesson.EvaluationDate = DateTime.Now;

                    db.Entry(traineeLesson).State = EntityState.Modified;
                    if (db.SaveChanges() <= 0)
                        message = message + traineeLessonResult.FirstOrDefault().Lesson.LessonName + " is not saved to the trainee. ";
                }
                if (message == "")
                {
                    return new OperationResult { IsSuccess = true, Message = "Trainee evaluation item are successfully saved." };
                }
                return new OperationResult { IsSuccess = false, Message = message };
            }
            catch (Exception e)
            {
                return new OperationResult { IsSuccess = false, Message = e.Message };
            }
        }

        public OperationResult AcceptEvaluationTemplate(int traineeId, int lessonId, bool isAccepted)
        {
            try
            {
                string message = "";
                PTSContext db = new PTSContext();

                //Save Trainee Lesson evaluation template agreement
                var traineeLessonResult = db.TraineeLessons.Where(TL => TL.LessonId == lessonId && TL.TraineeId == traineeId).ToList();

                if (traineeLessonResult.Count() > 0)
                {
                    TraineeLesson traineeLesson = traineeLessonResult.FirstOrDefault();
                    if (isAccepted)
                        traineeLesson.Status = "Accepted";
                    else
                        traineeLesson.Status = "Rejected";
                    traineeLesson.AgreedDate = DateTime.Now;

                    db.Entry(traineeLesson).State = EntityState.Modified;
                    if (db.SaveChanges() <= 0)
                        message = "Agreement to " + traineeLessonResult.FirstOrDefault().Lesson.LessonName + " evaluation template is not saved successfully.";
                }
                if (message == "")
                {
                    return new OperationResult { IsSuccess = true, Message = "You are successfully confirmed your evaluation template acceptance." };
                }
                return new OperationResult { IsSuccess = false, Message = message };
            }
            catch (Exception e)
            {
                return new OperationResult { IsSuccess = false, Message = e.Message };
            }
        }

        public TraineeLesson CloneEvaluationTemplate(int lessonId, int traineeId)
        {
            try
            {
                int sequence = 0;
                PTSContext db = new PTSContext();
                var traineeLessonList = db.TraineeLessons.Include(c => c.Lesson).AsNoTracking().
                  Where(TL => TL.TraineeId == traineeId && TL.LessonId == lessonId).ToList();

                TraineeLesson objTraineeLesson;

                if (traineeLessonList.Count > 0)
                {
                    //1. CLONE TRAINEE LESSON

                    var traineeLesson = traineeLessonList.OrderByDescending(TL => TL.Sequence).FirstOrDefault();
                    sequence = traineeLesson.Sequence + 1;

                    objTraineeLesson = new TraineeLesson();
                    objTraineeLesson.BatchCategoryId = traineeLesson.BatchCategoryId;
                    objTraineeLesson.Sequence = sequence;
                    objTraineeLesson.LessonId = traineeLesson.LessonId;

                    objTraineeLesson.TraineeId = traineeId;
                    objTraineeLesson.EvaluationTemplateId = traineeLesson.EvaluationTemplateId;

                    objTraineeLesson.StartDate = DateTime.Now;
                    objTraineeLesson.EndDate = DateTime.MaxValue;
                    objTraineeLesson.CreationDate = DateTime.Now;
                    objTraineeLesson.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    objTraineeLesson.RevisionDate = DateTime.Now;
                    objTraineeLesson.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                    objTraineeLesson.Remark = "";

                    db.TraineeLessons.Add(objTraineeLesson);
                    db.SaveChanges();
                }
                else
                {
                    var lesson = db.Lessons.Find(lessonId);
                    var traineeBatchClass = db.TraineeBatchClasses.Where(tr => tr.TraineeId == traineeId).FirstOrDefault();
                    var batchLesson = db.BatchLessons
                        .Where(bc=>bc.BatchCategory.BatchId == traineeBatchClass.BatchClass.BatchId
                        && bc.LessonId == lesson.LessonId).SingleOrDefault();
                    sequence = 1;

                    objTraineeLesson = new TraineeLesson();
                    objTraineeLesson.BatchCategoryId = batchLesson.BatchCategoryId;
                    objTraineeLesson.Sequence = sequence;
                    objTraineeLesson.LessonId = batchLesson.LessonId;

                    objTraineeLesson.TraineeId = traineeId;
                    objTraineeLesson.EvaluationTemplateId = batchLesson.EvaluationTemplateId;

                    objTraineeLesson.StartDate = DateTime.Now;
                    objTraineeLesson.EndDate = DateTime.MaxValue;
                    objTraineeLesson.CreationDate = DateTime.Now;
                    objTraineeLesson.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    objTraineeLesson.RevisionDate = DateTime.Now;
                    objTraineeLesson.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                    objTraineeLesson.Remark = "";

                    db.TraineeLessons.Add(objTraineeLesson);
                    db.SaveChanges();

                }

                return objTraineeLesson;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public List<TraineeLesson> selectTraineeEvaluationTemplate(int lessonId, int traineeId)
        {
            List<TraineeLesson> traineeLesson = new List<TraineeLesson>();
            traineeLesson = dbContext.TraineeLessons.Where(ev => ev.TraineeId == traineeId && ev.LessonId == lessonId).ToList();
            return traineeLesson;
        }
        
      
        public bool PopulateEvaluationItemForTraineeLesson(int traineeId, int lessonId, int sequence)
        {
            try
            {
                var result = dbContext.TraineeLessons.Where(ev => ev.LessonId == lessonId
                   && ev.TraineeId == traineeId
                   && ev.Sequence == sequence).ToList();
                var traineeLesson = result.FirstOrDefault();

                List<EvaluationCategory> evaluationCategories = dbContext.EvaluationCategories.Include(c => c.EvaluationTemplate).
                    Where(m => m.EvaluationTemplateId == traineeLesson.EvaluationTemplateId).ToList();
                evaluationCategories.ForEach(c => AddEvaluationCategory(traineeLesson.TraineeLessonId, c.EvaluationCategoryId));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private object AddEvaluationCategory(int traineeLessonId, int evaluationCategoryId)
        {
            try
            {
                EvaluationCategory evaluationCategory = dbContext.EvaluationCategories
                    .Single(c => c.EvaluationCategoryId == evaluationCategoryId);

                TraineeEvaluationCategory traineeEvaluationCategory = new TraineeEvaluationCategory();
                traineeEvaluationCategory.TraineeLessonId = traineeLessonId;
                traineeEvaluationCategory.EvaluationCategoryName = evaluationCategory.EvaluationCategoryName;
                traineeEvaluationCategory.StartDate = DateTime.Now;
                traineeEvaluationCategory.EndDate = new DateTime(9999, 12, 31);
                traineeEvaluationCategory.CreationDate = DateTime.Now;
                traineeEvaluationCategory.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                traineeEvaluationCategory.RevisionDate = DateTime.Now;
                traineeEvaluationCategory.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                traineeEvaluationCategory.sequenceNo = evaluationCategory.sequenceNo;
                dbContext.TraineeEvaluationCategories.Add(traineeEvaluationCategory);
                dbContext.SaveChanges();

                TraineeEvaluationCategory tEvaluationCategory = dbContext.TraineeEvaluationCategories.FirstOrDefault(e => e.TraineeLessonId == traineeLessonId && e.EvaluationCategoryName == evaluationCategory.EvaluationCategoryName);
                List<EvaluationItem> evaluationItemList = dbContext.EvaluationItems.Where(e => e.EvaluationCategory.EvaluationCategoryId == evaluationCategoryId).ToList();
                evaluationItemList.ForEach(EI => AddEvaluationItem(tEvaluationCategory.TraineeEvaluationCategoryId, EI.EvaluationItemName));

                return new { status = true }; // Success
            }
            catch (Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }

        private object AddEvaluationItem(int traineeEvaluationCategoryId, string evaluationItemName)
        {
            try
            {
                TraineeEvaluationItem traineeEvaluationItem = new TraineeEvaluationItem();
                traineeEvaluationItem.TraineeEvaluationCategoryId = traineeEvaluationCategoryId;
                traineeEvaluationItem.EvaluationItemName = evaluationItemName;
                traineeEvaluationItem.StartDate = DateTime.Now;
                traineeEvaluationItem.EndDate = new DateTime(9999, 12, 31);
                traineeEvaluationItem.CreationDate = DateTime.Now;
                traineeEvaluationItem.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                traineeEvaluationItem.RevisionDate = DateTime.Now;
                traineeEvaluationItem.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                traineeEvaluationItem.sequenceNo = traineeEvaluationItem.sequenceNo;
                dbContext.TraineeEvaluationItems.Add(traineeEvaluationItem);
                dbContext.SaveChanges();

                return new { status = true }; // Success
            }
            catch (Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }
    }
}
