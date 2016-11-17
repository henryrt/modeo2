using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTH.Modeo2
{
    public partial class Form1 : Form
    {
        TransportationStudy study = new TransportationStudy();

        public Form1()
        {
            InitializeComponent();
            study.DataChanged += UpdateUI;

            
        }

        private void initButton_Click(object sender, EventArgs e)
        {
            study.Initialize(1000); // msecs

            //study.AddFilter(new ObjectiveFilter() { ObjectiveName = "#Late", LimitValue = 4 }.GetFilter);
        }

        private string SortedColumn;
        private ListSortDirection SortedColumnOrder;

        private bool updating = false;
        private void UpdateUI()
        {
            if (updating) return;
            updating = true;

            var names = study.ObjectiveNames;
            var objectives = study.Objectives;

            textBox1.Text = study.nObjectives.ToString();
            textBox2.Text = study.nSolutions.ToString();

            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(names.ToArray());
            comboBox1.SelectedItem = study.SortColumnName;

            UpdateGrid(objectives, study.SolutionGrid);
            //Console.WriteLine(study.solver.DataStore.GetEnumerable<ISolution>().ElementAt(0));
            UpdateChart(study);
            updating = false;
        }

        private void UpdateChart(TransportationStudy study)
        {
            var tradeoffSummary = study.TradeOff(study.Objectives[0], study.Objectives[1]);
            chart1.Series.Clear();

            var series1 = chart1.Series.Add("Pareto Boundary");
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            
            foreach (var t in tradeoffSummary)
            {
                series1.Points.AddXY(t.Objective1Value, t.Objective2Value);//, t.Count);
            }
            series1.Points[0].AxisLabel = "#Late";
            series1.Points[1].AxisLabel = "Cost $";
            chart1.Refresh();
        }

        private void UpdateGrid(List<IObjective> objectives, ArrayList solns)
        {
            //populate dataset for grid
            var table = new DataTable();

            if (table.Columns.Count == 0)
            {
                table.Columns.AddRange(
                      objectives.Select(obj => new DataColumn(obj.Name, obj.DataType)).ToArray()
                    );
            }

            table.Clear();
            var lineNum = 0;
            foreach (string[] line in solns)
            {
                if (lineNum > 0)
                {
                    var row = table.NewRow();
                    var i = 0;
                    foreach (var col in objectives)
                    {
                        row[col.Name] = double.Parse(line[i]);
                        i++;
                    }
                    table.Rows.Add(row);
                }
                lineNum++;
            }

            dataGridView1.DataSource = table;
            foreach (var obj in objectives)
            {
                var cellStyle = dataGridView1.Columns[obj.Name].DefaultCellStyle;
                cellStyle.Format = obj.Format;
                cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (SortedColumn != null) dataGridView1.Sort(dataGridView1.Columns[SortedColumn], SortedColumnOrder);

            dataGridView1.Refresh();
        }

        private string GridToString(ArrayList grid)
        {
            var buf = new StringBuilder();
            var i = 0;
            foreach (string[] row in grid)
            {
                for (var ix = 0; ix < row.Length; ix++) buf.AppendFormat("{0,-12}", row[ix]);
                buf.AppendLine(" " + i++);
            }

            return buf.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // save sort info
            SortedColumn = dataGridView1.SortedColumn?.Name;
            if (SortedColumn != null) SortedColumnOrder = (dataGridView1.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
            for (int i=0; i < 5; i++) study.Run();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (comboBox1.SelectedItem?.ToString() != "")
            //{
                study.SortColumnName = comboBox1.SelectedItem.ToString();
                UpdateUI();
            //}
        }

        private void analyze_Click(object sender, EventArgs e)
        {
            var result = study.Analyze(study.Objectives[0], study.Objectives[1]);

            //UpdateGrid(study.Objectives, result);

            //var tradeoff = study.TradeOff(study.Objectives[0], study.Objectives[1]);

            //foreach (var g in tradeoff)
            //{
            //    System.Console.WriteLine("{0} {1} {2}",g.Objective1Value, g.Objective2Value, g.Count);
            //}
        }
    }
}
