using PTSMSDAL.Access.Scheduling.References;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Scheduling.References
{
    public class EquipmentTypeLogic
    {

        EquipmentTypeAccess equipmentTypeAccess = new EquipmentTypeAccess();

        public List<EquipmentType> List()
        {
            return equipmentTypeAccess.List();
        }

        public EquipmentType Details(int id)
        {
            return equipmentTypeAccess.Details(id);
        }

        public bool Add(EquipmentType equipmentType)
        {
            return equipmentTypeAccess.Add(equipmentType);
        }

        public bool Revise(EquipmentType equipmentType)
        {
            return equipmentTypeAccess.Revise(equipmentType);
        }

        public bool Delete(int id)
        {
            return equipmentTypeAccess.Delete(id);
        }
    }
}
