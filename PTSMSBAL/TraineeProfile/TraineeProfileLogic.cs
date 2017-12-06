using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Others.View;
using PTSMSDAL.TraineeProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.TraineeProfile
{
    public class TraineeProfileLogic
    {
        TraineeProfileAccess traineeProfileAccess = new TraineeProfileAccess();
        public List<Lesson> TraineeLessons(int traineeId)
        {
            return traineeProfileAccess.TraineeLessons(traineeId);
        }
        public List<Course> TraineeCourses(int traineeId)
        {
            return traineeProfileAccess.TraineeCourses(traineeId);
        }

        public List<Module> TraineeModuleList(int courseId, int traineeId)
        {
            return traineeProfileAccess.TraineeModuleList(courseId, traineeId);
        }

        public List<TraineeModuleExam> TraineeModuleExamList(int moduleId, int traineeId)
        {
            return null;
            //return traineeProfileAccess.TraineeModuleExamList(moduleId, traineeId);
        }

        public TraineeModuleExam ModuleExamDetails(int traineeModuleExamId)
        {
            return traineeProfileAccess.ModuleExamDetails(traineeModuleExamId);
        }

        public List<TraineeCourseExam> TraineeCourseExamList(int courseId, int traineeId)
        {
            return null;
            //return traineeProfileAccess.TraineeCourseExamList(courseId, traineeId);
        }

        public TraineeCourseExam CourseExamDetails(int traineeCourseExamId)
        {
            return traineeProfileAccess.CourseExamDetails(traineeCourseExamId);
        }

        public TraineeProfileView Profile(string companyId)
        {
            try
            {
                PersonLogic personLogic = new PersonLogic();
                TraineeProfileView traineeProfile = new TraineeProfileView();
                PTSContext db = new PTSContext();
                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    traineeProfile.FirstName = person.FirstName;
                    traineeProfile.LastName = person.LastName;
                    traineeProfile.MiddleName = person.MiddleName;
                    traineeProfile.CompanyId = person.CompanyId;
                    traineeProfile.Sex = person.Sex;
                    traineeProfile.BirthDate = person.BirthDate;
                    traineeProfile.Nationality = person.Nationality;
                    traineeProfile.City = person.City;
                    traineeProfile.Email = person.Email;
                    traineeProfile.Phone = person.Phone;
                    traineeProfile.Position = person.Position;
                    traineeProfile.RegistrationDate = person.RegistrationDate;
                    traineeProfile.Status = person.Status;
                    traineeProfile.Address = person.Address;

                    /////////////////////////////Get trainee Batch Information/////////////////////

                    var traineeDetail = db.TraineeBatchClasses.Where(tb => tb.Trainee.Person.CompanyId == person.CompanyId).FirstOrDefault();

                    traineeProfile.Program = traineeDetail.BatchClass.Batch.Program.ProgramName;
                    traineeProfile.Batch = traineeDetail.BatchClass.Batch.BatchName;
                    traineeProfile.BatchClass = traineeDetail.BatchClass.BatchClassName;
                    //traineeProfile.Location = person.Location;
                }
                return traineeProfile;
            }
            catch (Exception ex)
            {
                return new TraineeProfileView();
            }
        }

        public bool UpdateProfile(string companyId, string phoneNumber, string email)
        {
            try
            {
                PersonLogic personLogic = new PersonLogic();
                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    person.Phone = phoneNumber;
                    person.Email = email;
                    return (bool)personLogic.Revise(person);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
