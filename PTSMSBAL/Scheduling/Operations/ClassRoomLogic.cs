using System.Collections.Generic;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Operations;

namespace PTSMSBAL.Logic.Scheduling.Operations
{
    public class ClassRoomLogic
    {
        ClassRoomAccess classRoomAccess = new ClassRoomAccess();

        public List<ClassRoom> List()
        {
            return classRoomAccess.List();
        }

        public ClassRoom Details(int id)
        {
            return classRoomAccess.Details(id);
        }

        public bool Add(ClassRoom classRoom)
        {
            classRoom.Status = "Active";
            classRoom.RevisionNo = 1;
            return classRoomAccess.Add(classRoom);
        }

        public bool Revise(ClassRoom classRoom)
        {
            ClassRoom room = classRoomAccess.Details(classRoom.ClassRoomId);
            return classRoomAccess.Revise(room);
        }

        public bool Delete(int id)
        {
            return classRoomAccess.Delete(id);
        }
    }
}