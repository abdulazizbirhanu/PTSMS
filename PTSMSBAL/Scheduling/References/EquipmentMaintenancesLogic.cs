using System;
using System.Collections.Generic;
using System.Linq;
using PTSMSDAL.Access.Scheduling.References;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.References;

namespace PTSMSBAL.Scheduling.References
{
    public class EquipmentMaintenancesLogic
    {
        private PTSContext db = new PTSContext();
        EquipmentMaintenanceAcess equipmentMaintenanceAcess = new EquipmentMaintenanceAcess();
        public List<EquipmentMaintenance> List()
        {
            return equipmentMaintenanceAcess.List();
        }
        public List<EquipmentMaintenance> List(string month, string year)
        {
            DateTime selectDate = DateTime.Now;
            if (!string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
            {
                selectDate = new DateTime(Convert.ToInt16(year), Convert.ToInt16(month), 1);
            }
            List<EquipmentMaintenance> equipmentMaintenances = db.EquipmentMaintenances.Where(e => selectDate >= e.ActualCalanderStartDate).Where(e => e.ActualCalanderEndDate >= selectDate).ToList();
            return equipmentMaintenances;
        }
        public EquipmentMaintenance Details(int? id)
        {
            return equipmentMaintenanceAcess.Details(id);
        }

        public object Add(EquipmentMaintenance equipmentMaintenance)
        {
            return equipmentMaintenanceAcess.Add(equipmentMaintenance);
        }

        public object Revise(EquipmentMaintenance equipmentMaintenance)
        {
            EquipmentMaintenance equipmentMaintenanceTobeEdited = Details(equipmentMaintenance.EquipmentMaintenanceId);

            equipmentMaintenanceTobeEdited.EquipmentMaintenanceId = equipmentMaintenance.EquipmentMaintenanceId;
            //equipmentMaintenanceTobeEdited.Equipment = equipmentMaintenance.Equipment;
            equipmentMaintenanceTobeEdited.MaintenanceName = equipmentMaintenance.MaintenanceName;
            equipmentMaintenanceTobeEdited.EventType = equipmentMaintenance.EventType;
            equipmentMaintenanceTobeEdited.ParameterType = equipmentMaintenance.ParameterType;
            equipmentMaintenanceTobeEdited.ScheduledMaintenanceHour = equipmentMaintenance.ScheduledMaintenanceHour;
            equipmentMaintenanceTobeEdited.ScheduledCalanderStartDate = equipmentMaintenance.ScheduledCalanderStartDate;
            equipmentMaintenanceTobeEdited.ScheduledCalanderEndDate = equipmentMaintenance.ScheduledCalanderEndDate;
            equipmentMaintenanceTobeEdited.Remark = equipmentMaintenance.Remark;
            equipmentMaintenanceTobeEdited.Status = equipmentMaintenance.Status;

            return equipmentMaintenanceAcess.Revise(equipmentMaintenanceTobeEdited);
        }

        public object Delete(EquipmentMaintenance equipmentMaintenance)
        {
            return equipmentMaintenanceAcess.Delete(equipmentMaintenance);
        }

        public object Dispose()
        {
            return equipmentMaintenanceAcess.Dispose();
        }

        public object ReviseUpdate(EquipmentMaintenance equipmentMaintenance)
        {
            return equipmentMaintenanceAcess.Revise(equipmentMaintenance);
        }
    }
}
