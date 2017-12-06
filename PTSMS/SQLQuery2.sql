select 
rcc.CourseId rGroupCourseId,
(case when jc.RevisionGroupId is null then jc.CourseId else jc.RevisionGroupId end) RevisionGroupId,
jm.*
from
REL_PROGRAMCATEGORY rcp,
REL_COURSECATEGORY rcc,
REL_COURSEMODULE rcm,

REL_TRAINEEPROGRAM tp,
REL_TRAINEECATEGORY tcat,
REL_TRAINEECOURSE tc,
REL_TRAINEEMODULE tm,

REF_PROGRAM jp,
CATEGORY jcat,
COURSE jc,
MODULE jm
where
	rcp.ProgramId=1
and rcc.ProgramCategoryId=rcp.ProgramCategoryId
and rcm.CourseCategoryId=rcc.CourseCategoryId

and tp.ProgramId=1
and tp.TraineeProgramId=1
and tcat.TraineeProgramId=tp.TraineeProgramId
and tc.TraineeCategoryId=tcat.TraineeCategoryId
and	tm.TraineeCourseId=tc.TraineeCourseId

and jp.ProgramId=tp.ProgramId
and jcat.CategoryId=tcat.CategoryId
and jc.CourseId=tc.CourseId
and jm.ModuleId=tm.ModuleId

and rcp.ProgramId=(case when jp.RevisionGroupId is null then jp.ProgramId else jp.RevisionGroupId end)
and rcp.CategoryId=(case when jcat.RevisionGroupId is null then jcat.CategoryId else jcat.RevisionGroupId end)
and rcc.CourseId=(case when jc.RevisionGroupId is null then jc.CourseId else jc.RevisionGroupId end)
and rcm.ModuleId=(case when jm.RevisionGroupId is null then jm.ModuleId else jm.RevisionGroupId end);



select
rcc.courseId,
(select Case when c.RevisionGroupId is null then c.CourseId else c.RevisionGroupId end from COURSE c where CourseId=rcc.CourseId) actualRevisionGroupId, 
rcc.* 
from REL_COURSECATEGORY rcc where CourseId in(23,24,25,26);

--change the faulty course category to use revision group id for course
update REL_COURSECATEGORY 
set CourseId=(select Case when c.RevisionGroupId is null then c.CourseId else c.RevisionGroupId end from COURSE c where CourseId=REL_COURSECATEGORY.CourseId)
where CourseId in(23,24,25,26);


select * from MODULE where ModuleId in
(
select ModuleId from REL_MODULE_INSTRUCTOR_SCHEDULE
);

select * from REL_MODULE_INSTRUCTOR_SCHEDULE where ModuleInstructorScheduleId=1002;

select * from INSTRUCTOR where InstructorId=1;

select * from PERSON where PersonId=28;

select * from MODULE where ModuleId=5;

select
moduleId, 
(select case when m.RevisionGroupId is null then m.ModuleId else m.RevisionGroupId end  from Module m where m.ModuleId=p.ModuleId) rGroupId 
from 
REL_MODULE_INSTRUCTOR_SCHEDULE p;






