using PTSMSDAL.Access.Dispatch.Master;
using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Dispatch.Master
{
   public class OperationAreaLogic
    {
        operationAreaAccess operationAreaAccess = new operationAreaAccess();

        public List<OperationArea> List()
        {
            return operationAreaAccess.List();
        }

        public OperationArea Details(int id)
        {
            return operationAreaAccess.Details(id);
        }

        public bool Add(OperationArea operationArea)
        {
            operationArea.Status = "Active";
            operationArea.RevisionNo = 1;
            return operationAreaAccess.Add(operationArea);
        }

        public bool Revise(OperationArea operationArea)
        {

            OperationArea d = (OperationArea)operationAreaAccess.Details(operationArea.OperationAreaId);
            return operationAreaAccess.Revise(d);
        }

        public bool Delete(int id)
        {
            return operationAreaAccess.Delete(id);
        }
    }
}

