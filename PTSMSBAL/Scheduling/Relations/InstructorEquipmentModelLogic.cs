using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Scheduling.Relations
{
   public class InstructorEquipmentModelLogic
    {

        InstructorEquipmentModelAccess instructorEquipmentModelAccess = new InstructorEquipmentModelAccess();
        public List<InstructorEquipmentModel> List()
        {
            return instructorEquipmentModelAccess.List();
        }

        public InstructorEquipmentModel Details(int id)
        {
            return instructorEquipmentModelAccess.Details(id);
        }

        public object Add(InstructorEquipmentModel instructorEquipmentModel)
        {
            return instructorEquipmentModelAccess.Add(instructorEquipmentModel);
        }

        public object Revise(InstructorEquipmentModel instructorEquipmentModel)
        {
            return instructorEquipmentModelAccess.Revise(instructorEquipmentModel);
        }

        public object Delete(int id)
        {
            return instructorEquipmentModelAccess.Delete(id);
        }
    }
}
