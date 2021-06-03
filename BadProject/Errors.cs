using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adv
{
    public class Errors
    { 
        public DateTime ErrDate { get; set; }
        public string ErrorMessage { get; set; }
        public Errors (DateTime errDate, string errMessage)
        {
            ErrDate = errDate;
            ErrorMessage = errMessage;

        }
    }
}
