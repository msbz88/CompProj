using System.Collections.Generic;

namespace CompProj.Models {
    public interface IComparisonProcessor {
        List<string> Execute(IWorkTable masterTable, IWorkTable testTable);
    }
}
