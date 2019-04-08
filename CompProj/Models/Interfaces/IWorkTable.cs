using System.Collections.Generic;
using System.Threading.Tasks;
using CompProj.Models.Interfaces;

namespace CompProj.Models {
    public interface IWorkTable {
        string Name { get; }
        Row Headers { get; }
        List<Row> Rows { get; }
        int ColumnsCount { get; }
        int RowsCount { get; }
        string Delimiter { get; }
        void SetGroupId(List<int> pivotKeys);
        //Task<Dictionary<int, HashSet<string>>> GetColumnsAsync();
        void LoadData(IEnumerable<string> data, string delimiter, bool isHeadersExist);
        void SaveToFile(string filePath);
    }
}
