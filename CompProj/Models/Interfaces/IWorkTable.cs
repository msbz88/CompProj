using System.Collections.Generic;

namespace CompProj.Models {
    public interface IWorkTable {
        Row Headers { get; }
        List<Row> Data { get; }
        int RowsCount { get; }
        int ColumnsCount { get; }
        void LoadDataAsync(List<string> data, char delimiter, bool isHeadersExist);
        List<string> GetColumn(int columnPosition);
    }
}
