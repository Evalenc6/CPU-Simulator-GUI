using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CpuSchedulingWinForms
{
    public partial class ComparisonForm : Form
    {
        public ComparisonForm(Dictionary<string, SimulationResult> results)
        {
            InitializeComponent();

            // Create ListView dynamically
            ListView listViewComparison = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                GridLines = true,
                FullRowSelect = true
            };

            listViewComparison.Columns.Add("Algorithm", 120);
            listViewComparison.Columns.Add("AWT", 80);
            listViewComparison.Columns.Add("ATT", 80);
            listViewComparison.Columns.Add("CPU Util.", 100);
            listViewComparison.Columns.Add("Throughput", 100);
            listViewComparison.Columns.Add("Resp. Time", 100);

            foreach (var entry in results)
            {
                var item = new ListViewItem(entry.Key);
                item.SubItems.Add(entry.Value.AWT.ToString("F2"));
                item.SubItems.Add(entry.Value.ATT.ToString("F2"));
                item.SubItems.Add(entry.Value.CPUUtilization.ToString("F2") + "%");
                item.SubItems.Add(entry.Value.Throughput.ToString("F2"));
                item.SubItems.Add(entry.Value.ResponseTime.ToString("F2"));

                listViewComparison.Items.Add(item);
            }

            this.Controls.Add(listViewComparison);
        }
    }
}
