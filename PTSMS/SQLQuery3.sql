--change the faulty course category to use revision group id for course
update REL_COURSECATEGORY 
set CourseId=(select Case when c.RevisionGroupId is null then c.CourseId else c.RevisionGroupId end from COURSE c where CourseId=REL_COURSECATEGORY.CourseId)
--where CourseId in(23,24,25,26);

--change the faulty associations which use non-revision group id
update REL_MODULE_INSTRUCTOR_SCHEDULE 
set ModuleId=(select Case when m.RevisionGroupId is null then m.ModuleId else m.RevisionGroupId end from Module m where ModuleId=REL_MODULE_INSTRUCTOR_SCHEDULE.ModuleId)
--where ModuleId in(82);


select * from Users where UserName like '%20930';