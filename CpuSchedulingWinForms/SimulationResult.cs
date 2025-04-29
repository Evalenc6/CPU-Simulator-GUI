using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuSchedulingWinForms
{
    public class SimulationResult
    {
        public double AWT { get; set; } 
        public double ATT { get; set; } 
        public double CPUUtilization { get; set; }
        public double Throughput { get; set; }
        public double ResponseTime { get; set; }

    }
}
