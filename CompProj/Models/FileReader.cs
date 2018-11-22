using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class FileReader: IFileReader {
        public Encoding Encoding { get; set; }
        public int BufferSize { get; set; }

        public FileReader(Encoding encoding, int bufferSize) {
            Encoding = encoding;
            BufferSize = bufferSize;
        }

        public async Task<List<string>> ReadAllLinesAsync(string filePath, int rowsToSkip) {
            var lines = new List<string>();
            FileOptions fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, fileOptions))
            using (var reader = new StreamReader(stream, Encoding)) {
                if (rowsToSkip > 0) {
                    for (int i = 0; i < rowsToSkip; i++) {
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

        public List<string> PreviewFile (string filePath, int rowsToTake) {
            return File.ReadAllLines(filePath).Take(rowsToTake).ToList();
        }
    }
}
