using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Operations;

namespace PTSMSBAL.Curriculum.Operations
{
    public class ExamLogic
    {
        ExamAccess examAccess = new ExamAccess();

        public object List()
        {
            return examAccess.List();
        }

        public object Details(int id)
        {
            return examAccess.Details(id);
        }
        public Exam ExamDetail(int courseExam, string examType)
        {
            return examAccess.ExamDetail(courseExam, examType);
        }

        public object Add(Exam exam)
        {
            exam.Status = "Active";
            exam.RevisionNo = 1;
            return examAccess.Add(exam);
        }

        public object Revise(Exam exam)
        {
            Exam exa = (Exam)examAccess.Details(exam.ExamId);
            exa.Status = "Replaced";

            exam.RevisionNo = exa.RevisionNo + 1;
            exam.Status = "Active";

            if (exa.RevisionGroupId == null)
                exam.RevisionGroupId = exam.ExamId;
            else
                exam.RevisionGroupId = exa.RevisionGroupId;

            examAccess.Revise(exa);

            return examAccess.Add(exam);
        }

        public object Delete(int id)
        {
            return examAccess.Delete(id);
        }
    }
}