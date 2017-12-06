using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.Relations;

namespace PTSMSBAL.Scheduling.Relations
{
    public class EquipmentCertificateLogic
    {
        EquipmentCertificateAccess equipmentCertificateAccess = new EquipmentCertificateAccess();   

        public List<EquipmentCertificate> List()
        {
            return equipmentCertificateAccess.List();
        }

        public EquipmentCertificate Details(int id)
        {
            return equipmentCertificateAccess.Details(id);
        }

        public object Add(EquipmentCertificate equipmentCertificate)
        {
            return equipmentCertificateAccess.Add(equipmentCertificate);
        }

        public object Revise(EquipmentCertificate equipmentCertificate)
        {
            return equipmentCertificateAccess.Revise(equipmentCertificate);
        }

        public object Delete(int id)
        {
            return equipmentCertificateAccess.Delete(id);
        }
    }
}
