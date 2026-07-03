using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.KavenegarServices.Dto
{
    public class KavenegarResponse<T>
    {
        public T Entries { get; set; }
        public ReturnInfo Return { get; set; }
    }
    public class ReturnInfo
    {
        public int Status { get; set; }
        public string Message { get; set; }
    }
}
