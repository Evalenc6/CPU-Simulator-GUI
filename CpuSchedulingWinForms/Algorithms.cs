using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpuSchedulingWinForms
{
    public static class Algorithms
    {
        /*Imma add these new algorithms at the top 
  starting with Shortest Remaining time first 
  then multi-level queue


  Shortest Remaining Time First:
    Preemptive version of the SRTF where the process
    with the remaining burst time left is ran
  Multilevel Queue:
    Processes are perminently assigned to queues based on
    their priority and each queue has its own scheduling 
    method and there is no movement between the queues. 
    Q1 sytem process (higher prio) | Q2 user processes (lower prio)
*/
        public static SimulationResult RunSRTFSimulation(List<Process> processes)
        {
            int n = processes.Count;
            int completed = 0, currentTime = 0;
            bool foundProcess = false;

            foreach (var p in processes)
                p.RemainingTime = p.BurstTime; // Reset RemainingTime

            double totalWaitingTime = 0, totalTurnaroundTime = 0, totalResponseTime = 0;

            while (completed != n)
            {
                int minRemainingTime = int.MaxValue;
                int shortest = -1;
                foundProcess = false;

                for (int i = 0; i < n; i++)
                {
                    if (processes[i].ArrivalTime <= currentTime &&
                        processes[i].RemainingTime > 0 &&
                        processes[i].RemainingTime < minRemainingTime)
                    {
                        minRemainingTime = processes[i].RemainingTime;
                        shortest = i;
                        foundProcess = true;
                    }
                }

                if (!foundProcess)
                {
                    currentTime++;
                    continue;
                }

                processes[shortest].RemainingTime--;

                if (processes[shortest].StartTime == -1)
                {
                    processes[shortest].StartTime = currentTime;
                }

                if (processes[shortest].RemainingTime == 0)
                {
                    completed++;
                    processes[shortest].CompletionTime = currentTime + 1;
                    processes[shortest].TurnaroundTime = processes[shortest].CompletionTime - processes[shortest].ArrivalTime;
                    processes[shortest].WaitingTime = processes[shortest].TurnaroundTime - processes[shortest].BurstTime;
                }

                currentTime++;
            }

            foreach (var p in processes)
            {
                totalWaitingTime += p.WaitingTime;
                totalTurnaroundTime += p.TurnaroundTime;
                totalResponseTime += p.StartTime - p.ArrivalTime;
            }

            SimulationResult result = new SimulationResult
            {
                AWT = totalWaitingTime / n,
                ATT = totalTurnaroundTime / n,
                ResponseTime = totalResponseTime / n,
                CPUUtilization = (double)(currentTime - processes.Sum(p => p.ArrivalTime)) / currentTime * 100,
                Throughput = (double)n / currentTime
            };

            return result;
        }
        public static SimulationResult RunMLQSimulation(List<Process> processes)
        {
            int n = processes.Count;

            List<Process> systemQueue = processes.Where(p => p.Priority <= 4).OrderBy(p => p.ArrivalTime).ToList();
            List<Process> userQueue = processes.Where(p => p.Priority > 4).OrderBy(p => p.ArrivalTime).ToList();

            List<Process> allProcesses = new List<Process>();
            allProcesses.AddRange(systemQueue);
            allProcesses.AddRange(userQueue);

            int currentTime = 0;
            double totalWaitingTime = 0, totalTurnaroundTime = 0, totalResponseTime = 0;

            foreach (var p in allProcesses)
            {
                if (currentTime < p.ArrivalTime)
                {
                    currentTime = p.ArrivalTime;
                }

                p.StartTime = currentTime;
                p.CompletionTime = currentTime + p.BurstTime;
                p.TurnaroundTime = p.CompletionTime - p.ArrivalTime;
                p.WaitingTime = p.StartTime - p.ArrivalTime;

                currentTime = p.CompletionTime;

                totalWaitingTime += p.WaitingTime;
                totalTurnaroundTime += p.TurnaroundTime;
                totalResponseTime += p.StartTime - p.ArrivalTime;
            }

            SimulationResult result = new SimulationResult
            {
                AWT = totalWaitingTime / n,
                ATT = totalTurnaroundTime / n,
                ResponseTime = totalResponseTime / n,
                CPUUtilization = (double)(currentTime - processes.Min(p => p.ArrivalTime)) / currentTime * 100,
                Throughput = (double)n / currentTime
            };

            return result;
        }

        public static void srtfAlgorithm(string userInput)
        {
            int np = Convert.ToInt16(userInput);
            List<Process> process = new List<Process>();

            for (int i = 0; i < np; i++)
            {
                string arrivalInput = Microsoft.VisualBasic.Interaction.InputBox(
                    $"Enter arrival time for process {i + 1}:",
                    "Arrival Time", "", -1, -1);
                string burstInput = Microsoft.VisualBasic.Interaction.InputBox(
                    $"Enter burst time for Process {i + 1}:",
                    "Burst Time", "", -1, -1);


                int arrivalTime = Convert.ToInt32(arrivalInput);
                int burstTime = Convert.ToInt32(burstInput);
                process.Add(new Process(i + 1, arrivalTime, burstTime));
            }
            int completed = 0, currentTime = 0, minRemainingTime = int.MaxValue, shortest = -1;
            bool foundProcess = false;

            while (completed != np)
            {
                minRemainingTime = int.MaxValue;  // reset for each time unit
                foundProcess = false;

                for (int i = 0; i < np; i++)
                {
                    if (process[i].ArrivalTime <= currentTime && process[i].RemainingTime > 0 && process[i].RemainingTime < minRemainingTime)
                    {
                        minRemainingTime = process[i].RemainingTime;
                        shortest = i;
                        foundProcess = true;
                    }
                }
                if (!foundProcess)
                {
                    currentTime++;
                    continue;
                }
                Console.WriteLine($"Time {currentTime}: Running P{process[shortest].ProcessId}");


                process[shortest].RemainingTime--;

                if (process[shortest].StartTime == -1)
                {
                    process[shortest].StartTime = currentTime;
                }

                minRemainingTime = process[shortest].RemainingTime == 0 ? int.MaxValue : process[shortest].RemainingTime;
                if (process[shortest].RemainingTime == 0)
                {
                    completed++;
                    process[shortest].CompletionTime = currentTime + 1;
                    process[shortest].TurnaroundTime = process[shortest].CompletionTime - process[shortest].ArrivalTime;
                    process[shortest].WaitingTime = process[shortest].TurnaroundTime - process[shortest].BurstTime;

                }

                currentTime++;
                foundProcess = false;
            }

            double totalWaitingTime = 0, totalTurnaroundTime = 0, totalResponseTime = 0;
            foreach (var p in process)
            {
                totalWaitingTime += p.WaitingTime;
                totalTurnaroundTime += p.TurnaroundTime;
                totalResponseTime += p.StartTime - p.ArrivalTime; // Response Time = Start Time - Arrival Time
            }

            double avgWaitingTime = totalWaitingTime / np;
            double avgTurnaroundTime = totalTurnaroundTime / np;
            double avgResponseTime = totalResponseTime / np;

            MessageBox.Show($"[SRTF Scheduling]\n" +
                            $"Average Waiting Time: {avgWaitingTime:F2} units\n" +
                            $"Average Turnaround Time: {avgTurnaroundTime:F2} units\n" +
                            $"Average Response Time: {avgResponseTime:F2} units",
                            "SRTF Results", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        public static void mlqAlgorithm(string userInput)
        {
            int np = Convert.ToInt16(userInput);
            List<Process> systemQueue = new List<Process>(); // High priority for system
            List<Process> userQueue = new List<Process>();   // Low priority for user

            // Input arrival, burst, and priority
            for (int i = 0; i < np; i++)
            {
                string arrivalInput = Microsoft.VisualBasic.Interaction.InputBox(
                    $"Enter arrival time for process {i + 1}:",
                    "Arrival Time", "", -1, -1);

                string burstInput = Microsoft.VisualBasic.Interaction.InputBox(
                    $"Enter burst time for process {i + 1}:",
                    "Burst Time", "", -1, -1);

                string priorityInput = Microsoft.VisualBasic.Interaction.InputBox(
                    $"Enter priority (0-9) for process {i + 1}:\n(0 = highest, 9 = lowest)",
                    "Priority", "", -1, -1);

                int arrivalTime = Convert.ToInt32(arrivalInput);
                int burstTime = Convert.ToInt32(burstInput);
                int priority = Convert.ToInt32(priorityInput);

                if (priority <= 4)
                    systemQueue.Add(new Process(i + 1, arrivalTime, burstTime, priority, 0)); // Queue 0
                else
                    userQueue.Add(new Process(i + 1, arrivalTime, burstTime, priority, 1));    // Queue 1
            }

            List<Process> allProcesses = new List<Process>();
            allProcesses.AddRange(systemQueue.OrderBy(p => p.ArrivalTime));
            allProcesses.AddRange(userQueue.OrderBy(p => p.ArrivalTime));

            int currentTime = 0;
            double totalWaitingTime = 0, totalTurnaroundTime = 0;

            foreach (var p in allProcesses)
            {
                if (currentTime < p.ArrivalTime)
                {
                    currentTime = p.ArrivalTime;
                }

                p.StartTime = currentTime;
                p.CompletionTime = currentTime + p.BurstTime;
                p.TurnaroundTime = p.CompletionTime - p.ArrivalTime;
                p.WaitingTime = p.StartTime - p.ArrivalTime;

                currentTime = p.CompletionTime;

                totalWaitingTime += p.WaitingTime;
                totalTurnaroundTime += p.TurnaroundTime;
            }

            double avgWaitingTime = totalWaitingTime / np;
            double avgTurnaroundTime = totalTurnaroundTime / np;

            MessageBox.Show($"[MLQ Scheduling]\n" +
                            $"Average Waiting Time: {avgWaitingTime:F2} units\n" +
                            $"Average Turnaround Time: {avgTurnaroundTime:F2} units",
                            "MLQ Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static SimulationResult RunFCFSSimulation(List<Process> processes)
        {
            int n = processes.Count;
            processes = processes.OrderBy(p => p.ArrivalTime).ToList();

            int currentTime = 0;
            double totalWaitingTime = 0, totalTurnaroundTime = 0, totalResponseTime = 0;

            foreach (var p in processes)
            {
                if (currentTime < p.ArrivalTime)
                {
                    currentTime = p.ArrivalTime;
                }

                p.StartTime = currentTime;
                p.CompletionTime = currentTime + p.BurstTime;
                p.TurnaroundTime = p.CompletionTime - p.ArrivalTime;
                p.WaitingTime = p.StartTime - p.ArrivalTime;

                currentTime = p.CompletionTime;

                totalWaitingTime += p.WaitingTime;
                totalTurnaroundTime += p.TurnaroundTime;
                totalResponseTime += p.StartTime - p.ArrivalTime;
            }

            return new SimulationResult
            {
                AWT = totalWaitingTime / n,
                ATT = totalTurnaroundTime / n,
                ResponseTime = totalResponseTime / n,
                CPUUtilization = (double)(currentTime - processes.Min(p => p.ArrivalTime)) / currentTime * 100,
                Throughput = (double)n / currentTime
            };
        }


        public static void fcfsAlgorithm(string userInput)
        {
            int np = Convert.ToInt16(userInput);
            int npX2 = np * 2;

            double[] bp = new double[np];
            double[] wtp = new double[np];
            string[] output1 = new string[npX2];
            double twt = 0.0, awt; 
            int num;

            DialogResult result = MessageBox.Show("First Come First Serve Scheduling ", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                for (num = 0; num <= np - 1; num++)
                {
                    //MessageBox.Show("Enter Burst time for P" + (num + 1) + ":", "Burst time for Process", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    //Console.WriteLine("\nEnter Burst time for P" + (num + 1) + ":");

                    string input =
                    Microsoft.VisualBasic.Interaction.InputBox("Enter Burst time: ",
                                                       "Burst time for P" + (num + 1),
                                                       "",
                                                       -1, -1);

                    bp[num] = Convert.ToInt64(input);

                    //var input = Console.ReadLine();
                    //bp[num] = Convert.ToInt32(input);
                }

                for (num = 0; num <= np - 1; num++)
                {
                    if (num == 0)
                    {
                        wtp[num] = 0;
                    }
                    else
                    {
                        wtp[num] = wtp[num - 1] + bp[num - 1];
                        MessageBox.Show("Waiting time for P" + (num + 1) + " = " + wtp[num], "Job Queue", MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
                }
                for (num = 0; num <= np - 1; num++)
                {
                    twt = twt + wtp[num];
                }
                awt = twt / np;
                MessageBox.Show("Average waiting time for " + np + " processes" + " = " + awt + " sec(s)", "Average Awaiting Time", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            else if (result == DialogResult.No)
            {
                //this.Hide();
                //Form1 frm = new Form1();
                //frm.ShowDialog();
            }
        }
        public static SimulationResult RunSJFSimulation(List<Process> processes)
        {
            int n = processes.Count;
            processes = processes.OrderBy(p => p.BurstTime).ThenBy(p => p.ArrivalTime).ToList();

            int currentTime = 0;
            double totalWaitingTime = 0, totalTurnaroundTime = 0, totalResponseTime = 0;

            foreach (var p in processes)
            {
                if (currentTime < p.ArrivalTime)
                {
                    currentTime = p.ArrivalTime;
                }

                p.StartTime = currentTime;
                p.CompletionTime = currentTime + p.BurstTime;
                p.TurnaroundTime = p.CompletionTime - p.ArrivalTime;
                p.WaitingTime = p.StartTime - p.ArrivalTime;

                currentTime = p.CompletionTime;

                totalWaitingTime += p.WaitingTime;
                totalTurnaroundTime += p.TurnaroundTime;
                totalResponseTime += p.StartTime - p.ArrivalTime;
            }

            return new SimulationResult
            {
                AWT = totalWaitingTime / n,
                ATT = totalTurnaroundTime / n,
                ResponseTime = totalResponseTime / n,
                CPUUtilization = (double)(currentTime - processes.Min(p => p.ArrivalTime)) / currentTime * 100,
                Throughput = (double)n / currentTime
            };
        }


        public static void sjfAlgorithm(string userInput)
        {
            int np = Convert.ToInt16(userInput);

            double[] bp = new double[np];
            double[] wtp = new double[np];
            double[] p = new double[np];
            double twt = 0.0, awt; 
            int x, num;
            double temp = 0.0;
            bool found = false;

            DialogResult result = MessageBox.Show("Shortest Job First Scheduling", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                for (num = 0; num <= np - 1; num++)
                {
                    string input =
                        Microsoft.VisualBasic.Interaction.InputBox("Enter burst time: ",
                                                           "Burst time for P" + (num + 1),
                                                           "",
                                                           -1, -1);

                    bp[num] = Convert.ToInt64(input);
                }
                for (num = 0; num <= np - 1; num++)
                {
                    p[num] = bp[num];
                }
                for (x = 0; x <= np - 2; x++)
                {
                    for (num = 0; num <= np - 2; num++)
                    {
                        if (p[num] > p[num + 1])
                        {
                            temp = p[num];
                            p[num] = p[num + 1];
                            p[num + 1] = temp;
                        }
                    }
                }
                for (num = 0; num <= np - 1; num++)
                {
                    if (num == 0)
                    {
                        for (x = 0; x <= np - 1; x++)
                        {
                            if (p[num] == bp[x] && found == false)
                            {
                                wtp[num] = 0;
                                MessageBox.Show("Waiting time for P" + (x + 1) + " = " + wtp[num], "Waiting time:", MessageBoxButtons.OK, MessageBoxIcon.None);
                                //Console.WriteLine("\nWaiting time for P" + (x + 1) + " = " + wtp[num]);
                                bp[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                    else
                    {
                        for (x = 0; x <= np - 1; x++)
                        {
                            if (p[num] == bp[x] && found == false)
                            {
                                wtp[num] = wtp[num - 1] + p[num - 1];
                                MessageBox.Show("Waiting time for P" + (x + 1) + " = " + wtp[num], "Waiting time", MessageBoxButtons.OK, MessageBoxIcon.None);
                                //Console.WriteLine("\nWaiting time for P" + (x + 1) + " = " + wtp[num]);
                                bp[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                }
                for (num = 0; num <= np - 1; num++)
                {
                    twt = twt + wtp[num];
                }
                MessageBox.Show("Average waiting time for " + np + " processes" + " = " + (awt = twt / np) + " sec(s)", "Average waiting time", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public static SimulationResult RunPrioritySimulation(List<Process> processes)
        {
            int n = processes.Count;
            processes = processes.OrderBy(p => p.Priority).ThenBy(p => p.ArrivalTime).ToList();

            int currentTime = 0;
            double totalWaitingTime = 0, totalTurnaroundTime = 0, totalResponseTime = 0;

            foreach (var p in processes)
            {
                if (currentTime < p.ArrivalTime)
                {
                    currentTime = p.ArrivalTime;
                }

                p.StartTime = currentTime;
                p.CompletionTime = currentTime + p.BurstTime;
                p.TurnaroundTime = p.CompletionTime - p.ArrivalTime;
                p.WaitingTime = p.StartTime - p.ArrivalTime;

                currentTime = p.CompletionTime;

                totalWaitingTime += p.WaitingTime;
                totalTurnaroundTime += p.TurnaroundTime;
                totalResponseTime += p.StartTime - p.ArrivalTime;
            }

            return new SimulationResult
            {
                AWT = totalWaitingTime / n,
                ATT = totalTurnaroundTime / n,
                ResponseTime = totalResponseTime / n,
                CPUUtilization = (double)(currentTime - processes.Min(p => p.ArrivalTime)) / currentTime * 100,
                Throughput = (double)n / currentTime
            };
        }

        public static void priorityAlgorithm(string userInput)
        {
            int np = Convert.ToInt16(userInput);

            DialogResult result = MessageBox.Show("Priority Scheduling ", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                double[] bp = new double[np];
                double[] wtp = new double[np + 1];
                int[] p = new int[np];
                int[] sp = new int[np];
                int x, num;
                double twt = 0.0;
                double awt;
                int temp = 0;
                bool found = false;
                for (num = 0; num <= np - 1; num++)
                {
                    string input =
                        Microsoft.VisualBasic.Interaction.InputBox("Enter burst time: ",
                                                           "Burst time for P" + (num + 1),
                                                           "",
                                                           -1, -1);

                    bp[num] = Convert.ToInt64(input);
                }
                for (num = 0; num <= np - 1; num++)
                {
                    string input2 =
                        Microsoft.VisualBasic.Interaction.InputBox("Enter priority: ",
                                                           "Priority for P" + (num + 1),
                                                           "",
                                                           -1, -1);

                    p[num] = Convert.ToInt16(input2);
                }
                for (num = 0; num <= np - 1; num++)
                {
                    sp[num] = p[num];
                }
                for (x = 0; x <= np - 2; x++)
                {
                    for (num = 0; num <= np - 2; num++)
                    {
                        if (sp[num] > sp[num + 1])
                        {
                            temp = sp[num];
                            sp[num] = sp[num + 1];
                            sp[num + 1] = temp;
                        }
                    }
                }
                for (num = 0; num <= np - 1; num++)
                {
                    if (num == 0)
                    {
                        for (x = 0; x <= np - 1; x++)
                        {
                            if (sp[num] == p[x] && found == false)
                            {
                                wtp[num] = 0;
                                MessageBox.Show("Waiting time for P" + (x + 1) + " = " + wtp[num], "Waiting time", MessageBoxButtons.OK);
                                //Console.WriteLine("\nWaiting time for P" + (x + 1) + " = " + wtp[num]);
                                temp = x;
                                p[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                    else
                    {
                        for (x = 0; x <= np - 1; x++)
                        {
                            if (sp[num] == p[x] && found == false)
                            {
                                wtp[num] = wtp[num - 1] + bp[temp];
                                MessageBox.Show("Waiting time for P" + (x + 1) + " = " + wtp[num], "Waiting time", MessageBoxButtons.OK);
                                //Console.WriteLine("\nWaiting time for P" + (x + 1) + " = " + wtp[num]);
                                temp = x;
                                p[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                }
                for (num = 0; num <= np - 1; num++)
                {
                    twt = twt + wtp[num];
                }
                MessageBox.Show("Average waiting time for " + np + " processes" + " = " + (awt = twt / np) + " sec(s)", "Average waiting time", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //Console.WriteLine("\n\nAverage waiting time: " + (awt = twt / np));
                //Console.ReadLine();
            }
            else
            {
                //this.Hide();
            }
        }
        public static SimulationResult RunRoundRobinSimulation(List<Process> processes, int timeQuantum = 2)
        {
            int n = processes.Count;
            foreach (var p in processes)
            {
                p.RemainingTime = p.BurstTime; // Reset remaining time
            }

            int currentTime = 0;
            double totalWaitingTime = 0, totalTurnaroundTime = 0, totalResponseTime = 0;
            Queue<Process> queue = new Queue<Process>(processes.OrderBy(p => p.ArrivalTime));

            while (queue.Count > 0)
            {
                Process current = queue.Dequeue();

                if (current.ArrivalTime > currentTime)
                {
                    currentTime = current.ArrivalTime;
                }

                if (current.StartTime == -1)
                {
                    current.StartTime = currentTime;
                }

                if (current.RemainingTime <= timeQuantum)
                {
                    currentTime += current.RemainingTime;
                    current.RemainingTime = 0;
                    current.CompletionTime = currentTime;
                    current.TurnaroundTime = current.CompletionTime - current.ArrivalTime;
                    current.WaitingTime = current.TurnaroundTime - current.BurstTime;
                }
                else
                {
                    currentTime += timeQuantum;
                    current.RemainingTime -= timeQuantum;
                    queue.Enqueue(current);
                }
            }

            foreach (var p in processes)
            {
                totalWaitingTime += p.WaitingTime;
                totalTurnaroundTime += p.TurnaroundTime;
                totalResponseTime += p.StartTime - p.ArrivalTime;
            }

            return new SimulationResult
            {
                AWT = totalWaitingTime / n,
                ATT = totalTurnaroundTime / n,
                ResponseTime = totalResponseTime / n,
                CPUUtilization = (double)(currentTime - processes.Min(p => p.ArrivalTime)) / currentTime * 100,
                Throughput = (double)n / currentTime
            };
        }

        public static void roundRobinAlgorithm(string userInput)
        {
            int np = Convert.ToInt16(userInput);
            int i, counter = 0;
            double total = 0.0;
            double timeQuantum;
            double waitTime = 0, turnaroundTime = 0;
            double averageWaitTime, averageTurnaroundTime;
            double[] arrivalTime = new double[10];
            double[] burstTime = new double[10];
            double[] temp = new double[10];
            int x = np;

            DialogResult result = MessageBox.Show("Round Robin Scheduling", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                for (i = 0; i < np; i++)
                {
                    string arrivalInput =
                            Microsoft.VisualBasic.Interaction.InputBox("Enter arrival time: ",
                                                               "Arrival time for P" + (i + 1),
                                                               "",
                                                               -1, -1);

                    arrivalTime[i] = Convert.ToInt64(arrivalInput);

                    string burstInput =
                            Microsoft.VisualBasic.Interaction.InputBox("Enter burst time: ",
                                                               "Burst time for P" + (i + 1),
                                                               "",
                                                               -1, -1);

                    burstTime[i] = Convert.ToInt64(burstInput);

                    temp[i] = burstTime[i];
                }
                string timeQuantumInput =
                            Microsoft.VisualBasic.Interaction.InputBox("Enter time quantum: ", "Time Quantum",
                                                               "",
                                                               -1, -1);

                timeQuantum = Convert.ToInt64(timeQuantumInput);
                Helper.QuantumTime = timeQuantumInput;

                for (total = 0, i = 0; x != 0;)
                {
                    if (temp[i] <= timeQuantum && temp[i] > 0)
                    {
                        total = total + temp[i];
                        temp[i] = 0;
                        counter = 1;
                    }
                    else if (temp[i] > 0)
                    {
                        temp[i] = temp[i] - timeQuantum;
                        total = total + timeQuantum;
                    }
                    if (temp[i] == 0 && counter == 1)
                    {
                        x--;
                        //printf("nProcess[%d]tt%dtt %dttt %d", i + 1, burst_time[i], total - arrival_time[i], total - arrival_time[i] - burst_time[i]);
                        MessageBox.Show("Turnaround time for Process " + (i + 1) + " : " + (total - arrivalTime[i]), "Turnaround time for Process " + (i + 1), MessageBoxButtons.OK);
                        MessageBox.Show("Wait time for Process " + (i + 1) + " : " + (total - arrivalTime[i] - burstTime[i]), "Wait time for Process " + (i + 1), MessageBoxButtons.OK);
                        turnaroundTime = (turnaroundTime + total - arrivalTime[i]);
                        waitTime = (waitTime + total - arrivalTime[i] - burstTime[i]);                        
                        counter = 0;
                    }
                    if (i == np - 1)
                    {
                        i = 0;
                    }
                    else if (arrivalTime[i + 1] <= total)
                    {
                        i++;
                    }
                    else
                    {
                        i = 0;
                    }
                }
                averageWaitTime = Convert.ToInt64(waitTime * 1.0 / np);
                averageTurnaroundTime = Convert.ToInt64(turnaroundTime * 1.0 / np);
                MessageBox.Show("Average wait time for " + np + " processes: " + averageWaitTime + " sec(s)", "", MessageBoxButtons.OK);
                MessageBox.Show("Average turnaround time for " + np + " processes: " + averageTurnaroundTime + " sec(s)", "", MessageBoxButtons.OK);
            }
        }
    }
}

