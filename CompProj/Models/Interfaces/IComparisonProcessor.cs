using System.Collections.Generic;

namespace CompProj.Models {
    public interface IComparisonProcessor {
        void Compare(IWorkTable masterTable, IWorkTable testTable);
    }
}
