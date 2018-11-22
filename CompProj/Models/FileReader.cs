using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class FileReader: IFileReader {
        public async Task<List<string>> ReadAllLinesAsync(string path, Encoding encoding, int bufferSize, int numRowsToSkip) {
            var lines = new List<string>();
            FileOptions fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions))
            using (var reader = new StreamReader(stream, encoding)) {
                if (numRowsToSkip > 0) {
                    for (int i = 0; i < numRowsToSkip; i++) {
                        await reader.ReadLineAsync();
                    }
                }
                string line;
                while ((line = await reader.ReadLineAsync()) != null) {
                    lines.Add(line);
                }
            }
            return lines.ToList();
        }
    }
}
