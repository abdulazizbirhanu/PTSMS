using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.InstructorProfile;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Others.View;
using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Relations;

namespace PTSMSBAL.InstructorProfile
{
    public class InstructorProfileLogic
    {
        InstructorProfileAccess instructorProfileAccess = new InstructorProfileAccess();
        public List<Lesson> InstructorLessons(int personId)
        {
            return instructorProfileAccess.InstructorLessons(personId);
        }

        public List<Course> InstructorCourses(int personId)
        {
            return instructorProfileAccess.InstructorCourses(personId);
        }

        public List<Module> ModuleList(int courseId, int personId)
        {
            return instructorProfileAccess.ModuleList(courseId, personId);
        }

        public List<ModuleExam> ModuleExamList(int moduleId, int personId)
        {
            return instructorProfileAccess.ModuleExamList(moduleId, personId);
        }

        public List<Batch> GetBatches(int personId)
        {
            return instructorProfileAccess.GetBatches(personId);
        }

        public InstructorProfileView Profile(string companyId)
        {
            try
            {
                PersonLogic personLogic = new PersonLogic();
                InstructorProfileView instructorProfile = new InstructorProfileView();
                PTSContext db = new PTSContext();
                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    instructorProfile.FirstName = person.FirstName;
                    instructorProfile.LastName = person.LastName;
                    instructorProfile.MiddleName = person.MiddleName;
                    instructorProfile.CompanyId = person.CompanyId;
                    instructorProfile.Sex = person.Sex;
                    instructorProfile.BirthDate = person.BirthDate;
                    instructorProfile.Nationality = person.Nationality;
                    instructorProfile.City = person.City;
                    instructorProfile.Email = person.Email;
                    instructorProfile.Phone = person.Phone;
                    instructorProfile.Position = person.Position;
                    instructorProfile.RegistrationDate = person.RegistrationDate;
                    instructorProfile.Status = person.Status;
                    instructorProfile.Address = person.Address;

                    /////////////////////////////Get instructor/////////////////////
                    var instructorQualificationProgram = db.InstructorQualifications.Where(Iq => Iq.InstructorId == person.PersonId).ToList();

                    string instQualification = "";
                    foreach (var qual in instructorQualificationProgram)
                    {
                        instQualification = instQualification + qual.QualificationType.Type + " - " + qual.QualificationType.Description + ", ";
                    }
                    instructorProfile.Qualification = instQualification;
                    //traineeProfile.Location = person.Location;
                }
                return instructorProfile;
            }
            catch (Exception ex)
            {
                return new InstructorProfileView();
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
