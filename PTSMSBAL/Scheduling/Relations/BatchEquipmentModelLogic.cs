using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Scheduling.Relations
{
  public  class BatchEquipmentModelLogic
    {
        BatchEquipmentModelAccess batchEquipmentModelAccess = new BatchEquipmentModelAccess();
        public List<BatchEquipmentModel> List()
        {
            return batchEquipmentModelAccess.List();
        }

        public BatchEquipmentModel Details(int id)
        {
            return batchEquipmentModelAccess.Details(id);
        }

        public object Add(BatchEquipmentModel batchEquipmentModel)
        {
            return batchEquipmentModelAccess.Add(batchEquipmentModel);
        }

        public object Revise(BatchEquipmentModel batchEquipmentModel)
        {
            return batchEquipmentModelAccess.Revise(batchEquipmentModel);
        }

        public object Delete(int id)
        {
            return batchEquipmentModelAccess.Delete(id);
        }
    }
}
