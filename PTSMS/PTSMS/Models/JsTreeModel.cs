using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTSMS.Models
{
    public class JsTreeModel
    {
        public string text;
        public JsTreeAttribute attr;
        // this was "open" but changing it to “leaf” adds “jstree-leaf” to the class   
        public string state = "leaf";

        public List<JsTreeModel> children;
    }
}