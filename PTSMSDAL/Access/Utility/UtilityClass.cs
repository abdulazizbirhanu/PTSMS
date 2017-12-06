using PTSMSDAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Utility
{
    public class UtilityClass
    {
        public int GetLatestIdNumber(string tableName)
        {
            PTSContext db = new PTSContext();
            return Convert.ToInt32(db.Database.SqlQuery<decimal>("Select IDENT_CURRENT ('" + tableName + "')", new object[0]).FirstOrDefault());
        }

        public string GetFileType(string fileType)
        {
            switch (fileType)
            {
                case ".doc":
                    fileType = "application/vnd.ms-word";
                    break;
                case ".docx":
                    fileType = "application/vnd.ms-word";
                    break;
                case ".ppt":
                    fileType = "application/vnd.ms-powerpointtd";
                    break;
                case ".pptx":
                    fileType = "application/vnd.ms-powerpointtd";
                    break;
                case ".xls":
                    fileType = "application/vnd.ms-excel";
                    break;
                case ".xlsx":
                    fileType = "application/vnd.ms-excel";
                    break;

                case ".jpg":
                    fileType = "image/jpg";
                    break;

                case ".png":
                    fileType = "image/png";
                    break;

                case ".gif":
                    fileType = "image/gif";
                    break;

                case ".txt":
                    fileType = "text/plain";
                    break;
                case ".html":
                    fileType = "text/HTML";
                    break;

                case ".pdf":
                    fileType = "application/pdf";
                    break;
            }
            return fileType;
        }

        /*public static bool GetUsePrivileges(string userName, string action)
         {

             bool found = false;
             foreach (ApplicationUserRole userRole in this.Roles)
             {
                 using (ApplicationDbContext _db = new ApplicationDbContext())
                 {
                     List<ApplicationRolePrivilege> rolePrivilege = _db.ApplicationRolePrivileges.Where(r => r.RoleId == userRole.RoleId).ToList();
                     foreach (var privilege in rolePrivilege)
                     {
                         found = _db.ApplicationPrivileges.Where(p => p.Action == requiredPrivilege && privilege.PrivilegeId == p.Id).ToList().Count > 0;
                         if (found)
                             break;
                     }
                 }
             }
             return found;

             return false;
         }*/
    }
}
