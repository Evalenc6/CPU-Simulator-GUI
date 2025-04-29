using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuSchedulingWinForms
{
    public class Process
    {
        public int ProcessId { get; set; }
        public int ArrivalTime { get; set; }
        public int BurstTime { get; set; }
        public int RemainingTime { get; set; }
        public int Priority { get; set; }
        public int QueueLevel { get; set; }

        public int StartTime { get; set; }
        public int CompletionTime { get; set; }
        public int WaitingTime { get; set; }
        public int TurnaroundTime { get; set; }

        public Process(int processId, int arrivalTime, int burstTime, int priority = 0, int queueLevel = 0)
        {
            ProcessId = processId;
            ArrivalTime = arrivalTime;
            BurstTime = burstTime;
            RemainingTime = burstTime;
            Priority = priority;
            QueueLevel = queueLevel;
            StartTime = -1;
        }

    }
}
