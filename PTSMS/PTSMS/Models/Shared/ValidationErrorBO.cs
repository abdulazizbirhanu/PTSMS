using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTSMS.Models.Shared
{
    public class ValidationErrorBO
    {
        public string ProcessStatus { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Exceptions { get; set; }
    }
}