select * from REL_TRAINEESYLLABUS where TraineeId=1656;

select * from REL_TRAINEEPROGRAM where TraineeProgramId=1;

select * from REL_TRAINEECATEGORY where TraineeProgramId=1;

select * from REL_TRAINEECOURSE where TraineeCategoryId=1;

select * from REL_TRAINEEMODULE where TraineeCourseId in
(
	select TraineeCourseId from REL_TRAINEECOURSE where TraineeCategoryId=1
);

select * from REL_TRAINEEBATCHCLASS where BatchClassId=1;





select * from BATCHCLASS where BatchId=1;

select * from BATCH;

select * from REL_TRAINEEBATCHCLASS;

select * from REL_MODULESCHEDULE where ModuleScheduleId=6;





select * from REL_TRAINEECATEGORY;

select * from REL_COURSECATEGORY where ProgramCategoryId=1;

select * from Course where courseId in
(select CourseId from REL_COURSECATEGORY where ProgramCategoryId=1)