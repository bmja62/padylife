using Common.GridResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Plans.DTOs
{
    public class GetAllPlanFilter : GlobalGrid
    {
        public long? UserId { get; set; }
    }
}
