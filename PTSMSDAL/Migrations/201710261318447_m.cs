namespace PTSMSDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m : DbMigration
    {
        public override void Up()
        {

            //DropPrimaryKey("dbo.REL_TRAINEEBATCHCLASS");

            AddColumn("dbo.REL_TRAINEEBATCHCLASS", "TraineeBatchClassId", c => c.Int(nullable: false, identity: true));
            //AddPrimaryKey("dbo.REL_TRAINEEBATCHCLASS", "TraineeBatchClassId");
            //DropColumn("dbo.REL_TRAINEEBATCHCLASS", "BatchTraineeId");

        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.REL_TRAINEEPREREQUISITE",
                c => new
                    {
                        TraineePrerequisiteId = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        PrerequisiteId = c.Int(nullable: false),
                        TraineeCourseId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedBy = c.String(),
                        RevisionDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RevisedBy = c.String(),
                    })
                .PrimaryKey(t => t.TraineePrerequisiteId);
            
            CreateTable(
                "dbo.REL_TRAINEEMODULEREFERENCE",
                c => new
                    {
                        TraineeModuleReferenceId = c.Int(nullable: false, identity: true),
                        TraineeModuleId = c.Int(nullable: false),
                        ReferenceId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedBy = c.String(),
                        RevisionDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RevisedBy = c.String(),
                    })
                .PrimaryKey(t => t.TraineeModuleReferenceId);
            
            CreateTable(
                "dbo.REL_TRAINEELESSONREFERENCE",
                c => new
                    {
                        TraineeLessonReferenceId = c.Int(nullable: false, identity: true),
                        TraineeLessonId = c.Int(nullable: false),
                        ReferenceId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedBy = c.String(),
                        RevisionDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RevisedBy = c.String(),
                    })
                .PrimaryKey(t => t.TraineeLessonReferenceId);
            
            CreateTable(
                "dbo.REL_TRAINEEEVALUATIONTEMPLATE",
                c => new
                    {
                        TraineeEvaluationTemplateId = c.Int(nullable: false, identity: true),
                        TraineeLessonId = c.Int(nullable: false),
                        EvaluationTemplateId = c.Int(nullable: false),
                        AggregatedScore = c.Single(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedBy = c.String(),
                        RevisionDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RevisedBy = c.String(),
                    })
                .PrimaryKey(t => t.TraineeEvaluationTemplateId);
            
            CreateTable(
                "dbo.REL_TRAINEECOURSEREFERENCE",
                c => new
                    {
                        TraineeCourseReferenceId = c.Int(nullable: false, identity: true),
                        TraineeCourseId = c.Int(nullable: false),
                        ReferenceId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedBy = c.String(),
                        RevisionDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RevisedBy = c.String(),
                    })
                .PrimaryKey(t => t.TraineeCourseReferenceId);
            
            CreateTable(
                "dbo.REL_TRAINEESYLLABUS",
                c => new
                    {
                        TraineeSyllabusId = c.Int(nullable: false, identity: true),
                        BatchId = c.Int(nullable: false),
                        TraineeId = c.Int(nullable: false),
                        SyllabusGeneratedDate = c.DateTime(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedBy = c.String(),
                        RevisionDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RevisedBy = c.String(),
                    })
                .PrimaryKey(t => t.TraineeSyllabusId);
            
            CreateTable(
                "dbo.REL_TRAINEEPROGRAM",
                c => new
                    {
                        TraineeProgramId = c.Int(nullable: false, identity: true),
                        TraineeSyllabusId = c.Int(nullable: false),
                        ProgramId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedBy = c.String(),
                        RevisionDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RevisedBy = c.String(),
                    })
                .PrimaryKey(t => t.TraineeProgramId);
            
            CreateTable(
                "dbo.REL_TRAINEECATEGORY",
                c => new
                    {
                        TraineeCategoryId = c.Int(nullable: false, identity: true),
                        TraineeProgramId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedBy = c.String(),
                        RevisionDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RevisedBy = c.String(),
                    })
                .PrimaryKey(t => t.TraineeCategoryId);
            
            CreateTable(
                "dbo.REL_BATCH_MODULE_SEQUENCE",
                c => new
                    {
                        BatchModuleSequenceId = c.Int(nullable: false, identity: true),
                        PhaseScheduleId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        Sequence = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.BatchModuleSequenceId);
            
            CreateTable(
                "dbo.REL_BATCH_LESSON_SEQUENCE",
                c => new
                    {
                        BatchLessonSequenceId = c.Int(nullable: false, identity: true),
                        PhaseScheduleId = c.Int(nullable: false),
                        LessonId = c.Int(nullable: false),
                        Sequence = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.BatchLessonSequenceId);
            
            CreateTable(
                "dbo.BATCHCLASSAUDIT",
                c => new
                    {
                        BatchClassId = c.Int(nullable: false, identity: true),
                        BatchId = c.Int(nullable: false),
                        BatchClassName = c.String(nullable: false, maxLength: 32),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedBy = c.String(),
                        RevisionDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RevisedBy = c.String(),
                    })
                .PrimaryKey(t => t.BatchClassId);
            
            CreateTable(
                "dbo.BATCHAUIDIT",
                c => new
                    {
                        BatchId = c.Int(nullable: false, identity: true),
                        ProgramId = c.Int(nullable: false),
                        BatchName = c.String(nullable: false, maxLength: 32),
                        BatchStartDate = c.DateTime(nullable: false),
                        EstimatedEndDate = c.DateTime(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedBy = c.String(),
                        RevisionDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RevisedBy = c.String(),
                    })
                .PrimaryKey(t => t.BatchId);
            
            AddColumn("dbo.REL_TRAINEELESSON", "TraineeCategoryId", c => c.Int(nullable: false));
            AddColumn("dbo.REL_TRAINEEEVALUATIONCATEGORY", "TraineeEvaluationTemplateId", c => c.Int(nullable: false));
            AddColumn("dbo.REL_TRAINEECOURSE", "TraineeCategoryId", c => c.Int(nullable: false));
            AddColumn("dbo.REL_TRAINEEBATCHCLASS", "BatchTraineeId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.BATCH_EQUIPMENT_MODEL", "BatchClassId", c => c.Int(nullable: false));
            AddColumn("dbo.REL_PHASESCHEDULE", "BatchClassId", c => c.Int(nullable: false));
            DropForeignKey("dbo.REL_TRAINEEEVALUATIONCATEGORY", "TraineeLessonId", "dbo.REL_TRAINEELESSON");
            DropForeignKey("dbo.REL_TRAINEELESSON", "TraineeId", "dbo.TRAINEE");
            DropForeignKey("dbo.REL_TRAINEELESSON", "EvaluationTemplateId", "dbo.EVALUATIONTEMPLATE");
            DropForeignKey("dbo.REL_TRAINEELESSON", "BatchCategoryId", "dbo.REL_BATCHCATEGORY");
            DropForeignKey("dbo.REL_TRAINEECOURSE", "TraineeId", "dbo.TRAINEE");
            DropForeignKey("dbo.REL_TRAINEECOURSE", "BatchCategoryId", "dbo.REL_BATCHCATEGORY");
            DropForeignKey("dbo.REL_BATCHCOURSEPREREQUISITE", "PrerequisiteId", "dbo.COURSE");
            DropForeignKey("dbo.REL_BATCHCOURSEPREREQUISITE", "CourseId", "dbo.COURSE");
            DropForeignKey("dbo.REL_BATCHCOURSEPREREQUISITE", "BatchCourseId", "dbo.REL_BATCHCOURSE");
            DropForeignKey("dbo.REL_BATCHMODULEEXAM", "ExamId", "dbo.EXAM");
            DropForeignKey("dbo.REL_BATCHMODULEEXAM", "BatchModuleId", "dbo.REL_BATCHMODULE");
            DropForeignKey("dbo.REL_BATCHMODULE", "PhaseId", "dbo.REF_PHASE");
            DropForeignKey("dbo.REL_BATCHMODULE", "ModuleId", "dbo.MODULE");
            DropForeignKey("dbo.REL_BATCHMODULE", "BatchCourseId", "dbo.REL_BATCHCOURSE");
            DropForeignKey("dbo.REL_BATCHLESSON", "PhaseId", "dbo.REF_PHASE");
            DropForeignKey("dbo.REL_BATCHLESSON", "LessonId", "dbo.LESSON");
            DropForeignKey("dbo.REL_BATCHLESSON", "EvaluationTemplateId", "dbo.EVALUATIONTEMPLATE");
            DropForeignKey("dbo.REL_BATCHLESSON", "BatchCategoryId", "dbo.REL_BATCHCATEGORY");
            DropForeignKey("dbo.BATCH_EQUIPMENT_MODEL", "BatchId", "dbo.BATCH");
            DropForeignKey("dbo.REL_BATCHCOURSEEXAM", "ExamId", "dbo.EXAM");
            DropForeignKey("dbo.REL_BATCHCOURSEEXAM", "BatchCourseId", "dbo.REL_BATCHCOURSE");
            DropForeignKey("dbo.REL_BATCHCOURSE", "CourseId", "dbo.COURSE");
            DropForeignKey("dbo.REL_BATCHCOURSE", "BatchCategoryId", "dbo.REL_BATCHCATEGORY");
            DropForeignKey("dbo.REL_BATCHCATEGORY", "CategoryId", "dbo.CATEGORY");
            DropForeignKey("dbo.REL_BATCHCATEGORY", "BatchId", "dbo.BATCH");
            DropForeignKey("dbo.REL_PHASESCHEDULE", "BatchId", "dbo.BATCH");
            DropIndex("dbo.REL_TRAINEELESSON", new[] { "EvaluationTemplateId" });
            DropIndex("dbo.REL_TRAINEELESSON", "UK_TraineeLesson");
            DropIndex("dbo.REL_TRAINEEEVALUATIONCATEGORY", "UK_TraineeEvaluationCategory");
            DropIndex("dbo.REL_TRAINEECOURSE", new[] { "TraineeId" });
            DropIndex("dbo.REL_TRAINEECOURSE", "UK_TraineeCourse");
            DropIndex("dbo.REL_COURSEMODULE", "UK_CourseModule");
            DropIndex("dbo.REL_BATCHCOURSEPREREQUISITE", "UK_BatchCoursePrerequisite");
            DropIndex("dbo.REL_BATCHMODULE", new[] { "PhaseId" });
            DropIndex("dbo.REL_BATCHMODULE", "UK_BatchModule");
            DropIndex("dbo.REL_BATCHMODULEEXAM", "UK_BatchModuleExam");
            DropIndex("dbo.REL_BATCHLESSON", new[] { "PhaseId" });
            DropIndex("dbo.REL_BATCHLESSON", new[] { "EvaluationTemplateId" });
            DropIndex("dbo.REL_BATCHLESSON", "UK_BatchCourse");
            DropIndex("dbo.BATCH_EQUIPMENT_MODEL", "UK_BATCH_EQUIPMENT_MODEL");
            DropIndex("dbo.REL_BATCHCOURSE", "UK_BatchCourse");
            DropIndex("dbo.REL_BATCHCOURSEEXAM", "UK_BatchCourse");
            DropIndex("dbo.REL_BATCHCATEGORY", "UK_BatchCategory");
            DropIndex("dbo.REL_PHASESCHEDULE", "UK_TraineeBatchId");
            DropPrimaryKey("dbo.REL_TRAINEEBATCHCLASS");
            AlterColumn("dbo.REL_COURSEMODULE", "PhaseId", c => c.Int());
            DropColumn("dbo.REL_TRAINEELESSON", "EvaluationTemplateId");
            DropColumn("dbo.REL_TRAINEELESSON", "BatchCategoryId");
            DropColumn("dbo.REL_TRAINEELESSON", "TraineeId");
            DropColumn("dbo.REL_TRAINEEEVALUATIONCATEGORY", "TraineeLessonId");
            DropColumn("dbo.REL_TRAINEECOURSE", "TraineeId");
            DropColumn("dbo.REL_TRAINEECOURSE", "BatchCategoryId");
            DropColumn("dbo.REL_TRAINEEBATCHCLASS", "TraineeBatchClassId");
            DropColumn("dbo.BATCH_EQUIPMENT_MODEL", "BatchId");
            DropColumn("dbo.REL_PHASESCHEDULE", "BatchId");
            DropTable("dbo.REL_BATCHCOURSEPREREQUISITE");
            DropTable("dbo.REL_BATCHMODULE");
            DropTable("dbo.REL_BATCHMODULEEXAM");
            DropTable("dbo.REL_BATCHLESSON");
            DropTable("dbo.REL_BATCHCOURSE");
            DropTable("dbo.REL_BATCHCOURSEEXAM");
            DropTable("dbo.REL_BATCHCATEGORY");
            AddPrimaryKey("dbo.REL_TRAINEEBATCHCLASS", "BatchTraineeId");
            CreateIndex("dbo.REL_TRAINEEPREREQUISITE", new[] { "CourseId", "PrerequisiteId", "TraineeCourseId" }, unique: true, name: "UK_TRAINEEPREREQUISITE");
            CreateIndex("dbo.REL_TRAINEEMODULEREFERENCE", new[] { "TraineeModuleId", "ReferenceId" }, unique: true, name: "UK_TraineeModuleReference");
            CreateIndex("dbo.REL_TRAINEELESSONREFERENCE", new[] { "TraineeLessonId", "ReferenceId" }, unique: true, name: "UK_TraineeLessonReference");
            CreateIndex("dbo.REL_TRAINEELESSON", new[] { "TraineeCategoryId", "LessonId", "Sequence" }, unique: true, name: "UK_TraineeLesson");
            CreateIndex("dbo.REL_TRAINEEEVALUATIONTEMPLATE", new[] { "TraineeLessonId", "EvaluationTemplateId" }, unique: true, name: "UK_TraineeEvaluationTemplate");
            CreateIndex("dbo.REL_TRAINEEEVALUATIONCATEGORY", new[] { "TraineeEvaluationTemplateId", "EvaluationCategoryName" }, unique: true, name: "UK_TraineeEvaluationCategory");
            CreateIndex("dbo.REL_TRAINEECOURSEREFERENCE", new[] { "TraineeCourseId", "ReferenceId" }, unique: true, name: "UK_TraineeCourseReference");
            CreateIndex("dbo.REL_TRAINEECOURSE", new[] { "TraineeCategoryId", "CourseId" }, unique: true, name: "UK_TraineeCourse");
            CreateIndex("dbo.REL_TRAINEESYLLABUS", new[] { "BatchId", "TraineeId" }, unique: true, name: "UK_TraineeSyllabus");
            CreateIndex("dbo.REL_TRAINEEPROGRAM", new[] { "TraineeSyllabusId", "ProgramId" }, unique: true, name: "UK_TraineeProgram");
            CreateIndex("dbo.REL_TRAINEECATEGORY", new[] { "TraineeProgramId", "CategoryId" }, unique: true, name: "UK_TraineeCategory");
            CreateIndex("dbo.REL_COURSEMODULE", new[] { "CourseCategoryId", "ModuleId", "PhaseId" }, unique: true, name: "UK_CourseModule");
            CreateIndex("dbo.REL_BATCH_MODULE_SEQUENCE", new[] { "PhaseScheduleId", "ModuleId" }, unique: true, name: "UK_REL_BATCH_MODULE_SEQUENCE");
            CreateIndex("dbo.REL_BATCH_LESSON_SEQUENCE", new[] { "PhaseScheduleId", "LessonId", "Sequence" }, unique: true, name: "UK_REL_BATCH_LESSON_SEQUENCE");
            CreateIndex("dbo.BATCH_EQUIPMENT_MODEL", new[] { "EquipmentModelId", "BatchClassId" }, unique: true, name: "UK_BATCH_EQUIPMENT_MODEL");
            CreateIndex("dbo.BATCHCLASSAUDIT", new[] { "BatchId", "BatchClassName" }, unique: true, name: "UK_BatchClassAudit");
            CreateIndex("dbo.BATCHAUIDIT", new[] { "ProgramId", "BatchName" }, unique: true, name: "UK_BatchAudit");
            CreateIndex("dbo.REL_PHASESCHEDULE", new[] { "BatchClassId", "PhaseId", "LessonCategoryTypeId" }, unique: true, name: "UK_TraineeBatchClassId");
            AddForeignKey("dbo.REL_TRAINEEPREREQUISITE", "TraineeCourseId", "dbo.REL_TRAINEECOURSE", "TraineeCourseId");
            AddForeignKey("dbo.REL_TRAINEEPREREQUISITE", "PrerequisiteId", "dbo.PREREQUISITE", "PrerequisiteId");
            AddForeignKey("dbo.REL_TRAINEEPREREQUISITE", "CourseId", "dbo.COURSE", "CourseId");
            AddForeignKey("dbo.REL_TRAINEEMODULEREFERENCE", "TraineeModuleId", "dbo.REL_TRAINEEMODULE", "TraineeModuleId");
            AddForeignKey("dbo.REL_TRAINEEMODULEREFERENCE", "ReferenceId", "dbo.REFERENCE", "ReferenceId");
            AddForeignKey("dbo.REL_TRAINEELESSONREFERENCE", "TraineeLessonId", "dbo.REL_TRAINEELESSON", "TraineeLessonId");
            AddForeignKey("dbo.REL_TRAINEELESSONREFERENCE", "ReferenceId", "dbo.REFERENCE", "ReferenceId");
            AddForeignKey("dbo.REL_TRAINEEEVALUATIONCATEGORY", "TraineeEvaluationTemplateId", "dbo.REL_TRAINEEEVALUATIONTEMPLATE", "TraineeEvaluationTemplateId");
            AddForeignKey("dbo.REL_TRAINEEEVALUATIONTEMPLATE", "TraineeLessonId", "dbo.REL_TRAINEELESSON", "TraineeLessonId");
            AddForeignKey("dbo.REL_TRAINEELESSON", "TraineeCategoryId", "dbo.REL_TRAINEECATEGORY", "TraineeCategoryId");
            AddForeignKey("dbo.REL_TRAINEEEVALUATIONTEMPLATE", "EvaluationTemplateId", "dbo.EVALUATIONTEMPLATE", "EvaluationTemplateId");
            AddForeignKey("dbo.REL_TRAINEECOURSEREFERENCE", "TraineeCourseId", "dbo.REL_TRAINEECOURSE", "TraineeCourseId");
            AddForeignKey("dbo.REL_TRAINEECOURSEREFERENCE", "ReferenceId", "dbo.REFERENCE", "ReferenceId");
            AddForeignKey("dbo.REL_TRAINEECOURSE", "TraineeCategoryId", "dbo.REL_TRAINEECATEGORY", "TraineeCategoryId");
            AddForeignKey("dbo.REL_TRAINEECATEGORY", "TraineeProgramId", "dbo.REL_TRAINEEPROGRAM", "TraineeProgramId");
            AddForeignKey("dbo.REL_TRAINEEPROGRAM", "TraineeSyllabusId", "dbo.REL_TRAINEESYLLABUS", "TraineeSyllabusId");
            AddForeignKey("dbo.REL_TRAINEESYLLABUS", "TraineeId", "dbo.TRAINEE", "TraineeId");
            AddForeignKey("dbo.REL_TRAINEESYLLABUS", "BatchId", "dbo.BATCH", "BatchId");
            AddForeignKey("dbo.REL_TRAINEEPROGRAM", "ProgramId", "dbo.REF_PROGRAM", "ProgramId");
            AddForeignKey("dbo.REL_TRAINEECATEGORY", "CategoryId", "dbo.CATEGORY", "CategoryId");
            AddForeignKey("dbo.REL_BATCH_MODULE_SEQUENCE", "PhaseScheduleId", "dbo.REL_PHASESCHEDULE", "PhaseScheduleId");
            AddForeignKey("dbo.REL_BATCH_MODULE_SEQUENCE", "ModuleId", "dbo.MODULE", "ModuleId");
            AddForeignKey("dbo.REL_BATCH_LESSON_SEQUENCE", "PhaseScheduleId", "dbo.REL_PHASESCHEDULE", "PhaseScheduleId");
            AddForeignKey("dbo.REL_BATCH_LESSON_SEQUENCE", "LessonId", "dbo.LESSON", "LessonId");
            AddForeignKey("dbo.BATCH_EQUIPMENT_MODEL", "BatchClassId", "dbo.BATCHCLASS", "BatchClassId");
            AddForeignKey("dbo.BATCHCLASSAUDIT", "BatchId", "dbo.BATCH", "BatchId");
            AddForeignKey("dbo.BATCHAUIDIT", "ProgramId", "dbo.REF_PROGRAM", "ProgramId");
            AddForeignKey("dbo.REL_PHASESCHEDULE", "BatchClassId", "dbo.BATCHCLASS", "BatchClassId");
        }
    }
}
