using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Others
{
    public class UserMenu
    {
        public UserMenu()
        {
        }
        public int Id { get; set; }
        public string LinkText { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string Roles { get; set; }
        
        /*
        var menus = new[]
                    {
                   new { LinkText="Home", ActionName="Index",ControllerName="Home",Roles="All"  },
                   new { LinkText="About", ActionName="About",ControllerName="Home",Roles="Anonymous"  },
                   new { LinkText="Contact", ActionName="Contact",ControllerName="Home",Roles="Anonymous"  },
                   new { LinkText="Dashboard", ActionName="Index",ControllerName="Dealer",Roles="Dealer"  },
                   new { LinkText="Dashboard", ActionName="Index",ControllerName="Admin",Roles="Administrator"  },
                   new { LinkText="Administration", ActionName="GetUsers",ControllerName="Admin",Roles="Administrator"  },
                   new { LinkText="My Profile", ActionName="GetDealerInfo",ControllerName="Dealer",Roles="Dealer,PublicUser"  },
                   new { LinkText="Products", ActionName="GetProducts",ControllerName="Product",Roles="Dealer,Administrator"  },
                   new { LinkText="Search", ActionName="SearchProducts",ControllerName="Product",Roles="PublicUser,Dealer,Administrator"  },
                   new { LinkText="Purchase History", ActionName="GetHistory",ControllerName="Product",Roles="PublicUser"  },

                };                
        */
    }
}
