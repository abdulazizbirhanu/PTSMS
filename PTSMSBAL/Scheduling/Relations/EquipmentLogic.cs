using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.References;

namespace PTSMSBAL.Scheduling.Relations
{
    public class EquipmentLogic
    {
        EquipmentAccess equipmentAccess = new EquipmentAccess();

        public List<Equipment> List()
        {
            return equipmentAccess.List();
        }

        public Equipment Details(int id)
        {
            return equipmentAccess.Details(id);
        }

        public object Add(Equipment equipment)
        {
            return equipmentAccess.Add(equipment);
        }

        public object Revise(Equipment equipment)
        {
            Equipment equipmentTobeEdited = equipmentAccess.Details(equipment.EquipmentId);
            equipmentTobeEdited.Building = equipment.Building;
            equipmentTobeEdited.Description = equipment.Description;
            equipmentTobeEdited.EquipmentModelId = equipment.EquipmentModelId;
            equipmentTobeEdited.RoomNo = equipment.RoomNo;
            equipmentTobeEdited.WorkingHours = equipment.WorkingHours;
            equipmentTobeEdited.NameOrSerialNo = equipment.NameOrSerialNo;
            equipmentTobeEdited.LocationId = equipment.LocationId;
            equipmentTobeEdited.EquipmentStatusId = equipment.EquipmentStatusId;
            equipmentTobeEdited.StartTime = equipment.StartTime;
           
            return equipmentAccess.Revise(equipmentTobeEdited);
        }

        public object Delete(int id)
        {
            return equipmentAccess.Delete(id);
        }
    }
}
