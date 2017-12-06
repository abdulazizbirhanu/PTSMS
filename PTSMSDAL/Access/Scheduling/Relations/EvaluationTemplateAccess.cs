using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling.Relations
{
    public class EvaluationTemplateAccess
    {        
        public List<EquipmentView> get_EquipmentList()
        {
            //select Equipment it's certificate is not expired
            try
            {
                PTSContext db = new PTSContext();
                List<EquipmentView> equipmentList = new List<EquipmentView>();
                DateTime today = DateTime.Now;

                var result = (from E in db.Equipments
                              join EC in db.EquipmentCertificates on E.EquipmentId equals EC.EquipmentId
                              where EC.EndingDate > today

                              select new EquipmentView
                              {
                                  EquipmentId = E.EquipmentId
                              }).ToList();

                var equipments = result.Distinct().ToList();
                foreach (var equipment in equipments)
                {
                    equipmentList.Add(equipment);
                }
                return equipmentList;
            }
            catch (Exception)
            {

                return new List<EquipmentView>();
            }

        }

        
    }
}
