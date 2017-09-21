using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bathroom_checker_client
{
    public class RestroomStatus
    {
        public bool Occupied { get; set; }
        public DateTime OccupiedOn { get; set; } = DateTime.MinValue;
        public string OccupiedBy { get; set; }
        public Queue<string> Queue { get; set; } = new Queue<string>();
    }
}
