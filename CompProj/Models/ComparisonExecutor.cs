using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class ComparisonExecutor {
        IWorkTable MasterTable { get; set; }
        IWorkTable TestTable { get; set; }
        IWorkTable ComparisonTable { get; set; }
        IComparisonProcessor ComparisonProccessor { get; set; }

        public ComparisonExecutor(IWorkTable masterTable, IWorkTable testTable, IComparisonProcessor comparisonProcessor) {
            MasterTable = masterTable;
            TestTable = testTable;
            ComparisonProccessor = comparisonProcessor;
        }

        public void ExecuteComparison() {
            ComparisonProccessor.Compare(MasterTable, TestTable);
        }
    }
}
