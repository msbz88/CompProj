using System.Collections.Generic;

namespace CompProj.Models {
    public interface IWorkTable {
        string Name { get; }
        Row Headers { get; }
        List<Row> Rows { get; }
        int RowsCount { get; }
        int ColumnsCount { get; }
        void LoadDataAsync(List<string> data, char delimiter, bool isHeadersExist);
        void SaveToFile(string filePath);
        void ApplyRowNumberInGroup(List<int> compKeys);
        List<string> GetColumn(int columnPosition);
    }
}
