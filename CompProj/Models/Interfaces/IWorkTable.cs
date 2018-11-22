using System.Collections.Generic;

namespace CompProj.Models {
    public interface IWorkTable {
        List<string> Headers { get; }
        List<Row> Data { get; }
        void LoadDataAsync(List<string> data, char delimiter, bool isHeadersExist);
    }
}
