using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.KavenegarServices.Dto
{
    public class SendResult
    {
        public long? Messageid { get; set; }
        public int? Status { get; set; }
        public string Statustext { get; set; }
        public string Receptor { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
    }

}
