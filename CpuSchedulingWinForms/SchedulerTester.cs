using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpuSchedulingWinForms
{
    internal class SchedulerTester
    {
        public static void RunComparison(int numberOfProcesses)
        {
            List<Process> randomProcesses = GenerateRandomProcesses(numberOfProcesses);

            var fcfs = Algorithms.RunFCFSSimulation(CloneProcessList(randomProcesses));
            var sjf = Algorithms.RunSJFSimulation(CloneProcessList(randomProcesses));
            var priority = Algorithms.RunPrioritySimulation(CloneProcessList(randomProcesses));
            var rr = Algorithms.RunRoundRobinSimulation(CloneProcessList(randomProcesses), 2);
            var srtf = Algorithms.RunSRTFSimulation(CloneProcessList(randomProcesses));
            var mlq = Algorithms.RunMLQSimulation(CloneProcessList(randomProcesses));

            // Output Comparison
            MessageBox.Show($"[Comparison Results]\n\n" +
                            $"Algorithm         AWT     ATT     CPU Util.   Throughput  Resp. Time\n" +
                            $"--------------------------------------------------------------------------\n" +
                            $"FCFS          {fcfs.AWT:F2}   {fcfs.ATT:F2}    {fcfs.CPUUtilization:F2}%       {fcfs.Throughput:F2}          {fcfs.ResponseTime:F2}\n" +
                            $"SJF           {sjf.AWT:F2}   {sjf.ATT:F2}    {sjf.CPUUtilization:F2}%       {sjf.Throughput:F2}          {sjf.ResponseTime:F2}\n" +
                            $"Priority      {priority.AWT:F2}   {priority.ATT:F2}    {priority.CPUUtilization:F2}%       {priority.Throughput:F2}          {priority.ResponseTime:F2}\n" +
                            $"RoundRobin    {rr.AWT:F2}   {rr.ATT:F2}    {rr.CPUUtilization:F2}%       {rr.Throughput:F2}          {rr.ResponseTime:F2}\n" +
                            $"SRTF          {srtf.AWT:F2}   {srtf.ATT:F2}    {srtf.CPUUtilization:F2}%       {srtf.Throughput:F2}          {srtf.ResponseTime:F2}\n" +
                            $"MLQ           {mlq.AWT:F2}   {mlq.ATT:F2}    {mlq.CPUUtilization:F2}%       {mlq.Throughput:F2}          {mlq.ResponseTime:F2}\n",
                            "Scheduler Comparison", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static List<Process> GenerateRandomProcesses(int count)
        {
            Random rand = new Random();
            List<Process> processes = new List<Process>();
            for (int i = 0; i < count; i++)
            {
                int arrivalTime = rand.Next(0, 10);  // Arrives between time 0-10
                int burstTime = rand.Next(1, 10);    // Needs between 1-10 time units
                int priority = rand.Next(0, 10);     // Priority from 0 to 9
                processes.Add(new Process(i + 1, arrivalTime, burstTime, priority));
            }
            return processes;
        }

        private static List<Process> CloneProcessList(List<Process> original)
        {
            return original.Select(p => new Process(p.ProcessId, p.ArrivalTime, p.BurstTime, p.Priority)).ToList();
        }

    }
}
