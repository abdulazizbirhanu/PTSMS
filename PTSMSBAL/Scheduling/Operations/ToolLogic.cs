using System.Collections.Generic;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Operations;

namespace PTSMSBAL.Logic.Scheduling.Operations
{
    public class ToolLogic
    {
        ToolAccess toolAccess = new ToolAccess();

        public List<Tool> List()
        {
            return toolAccess.List();
        }

        public Tool Details(int id)
        {
            return toolAccess.Details(id);
        }

        public object Add(Tool tool)
        {
            return toolAccess.Add(tool);
        }

        public object Revise(Tool tool)
        {
            return toolAccess.Revise(tool);
        }

        public object Delete(int id)
        {
            return toolAccess.Delete(id);
        }
    }
}