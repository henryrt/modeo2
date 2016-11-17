using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class ResultTable
    {
        public IEnumerable<IObjective> Objectives;
        public List<ResultRow> Rows;

        public ResultTable(BaseSolver solver) : this(solver.DataStore.GetEnumerable<IObjective>(), solver.DataStore.GetEnumerable<ISolution>())
        {                
        }
        public ResultTable(IEnumerable<IObjective> objs, IEnumerable<ISolution> solns)
        {
            Objectives = objs;
            Rows = new List<ResultRow>();
            var rownum = 0;
            foreach (var s in solns)
            {
                ResultRow row = null;
                Rows.Add(row = new ResultRow()
                {
                    Solution = s,
                    RowNum = rownum++,
                    Cells = new List<Cell>()
                });
                foreach (var obj in objs)
                {
                    row.Cells.Add(new Cell()
                    {
                        Objective = obj,
                        Value = obj.Value(s)
                    });
                }
            }
        }
    }

    public class ResultRow
    {
        public int RowNum;
        public List<Cell> Cells;
        public ISolution Solution;
    }
    public class Cell
    {
        public IObjective Objective;
        public double Value;
    }
}
