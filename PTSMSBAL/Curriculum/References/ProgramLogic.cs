using PTSMSDAL.Access.Curriculum.References;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Curriculum.View;
using System.Collections.Generic;

namespace PTSMSBAL.Curriculum.References
{
    public class ProgramLogic
    {
        ProgramAccess programAccess = new ProgramAccess();


        // relation methods
        public object ListCategory(int ProgramId)
        {
            return programAccess.ListCategory(ProgramId);
        }

        public object AddCategory(int programId, List<string> categoryIdList)
        {
            return programAccess.AddCategory(programId, categoryIdList);
        }

        public object RemoveCategory(int categoryId)
        {
            return programAccess.RemoveCategory(categoryId);
        }

        public List<Program> List()
        {
            return programAccess.List();
        }

        public Program Details(int id)
        {
            return programAccess.Details(id);
        }

        public bool Add(Program program)
        {
            program.Status = "Active";
            program.RevisionNo = 1;
            return programAccess.Add(program);
        }

        public bool Revise(Program program)
        {
            Program prog = (Program)programAccess.Details(program.ProgramId);
            return programAccess.Revise(prog);
        }

        public bool Delete(int id)
        {
            return programAccess.Delete(id);
        }

        public ProgramHierarchy GetProgramHierarchy(int programId)
        {
            return programAccess.GetProgramHierarchy(programId);
        }
    }
}