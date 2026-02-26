using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Persistence
{
    public class ProcessedEvent
    {
        public Guid EventId { get; set; }
        public DateTime ProcessedAt { get; set; }
    }

}
