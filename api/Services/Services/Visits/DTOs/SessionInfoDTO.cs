using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Visits.DTOs
{
    public class SessionInfoDTO
    {
        public string VisitorId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
        public string LoginTime { get; set; }
    }
}
