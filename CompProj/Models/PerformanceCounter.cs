using System;
using System.Diagnostics;

namespace CompProj.Models {
    public class PerformanceCounter {
        private Stopwatch Watch { get; set; }
        public long StartMemory { get; private set; }
        public long UsedMemory { get; private set; }

        public void Start() {
            Watch = Stopwatch.StartNew();
            StartMemory = GC.GetTotalMemory(true);
        }

        public void Stop() {
            Watch.Stop();
            var elapsedMs = Watch.ElapsedMilliseconds;
            UsedMemory = GC.GetTotalMemory(true);
        }

        public double ConvertBytesToMegabytes(long bytes) {
            return Math.Round((bytes / 1024f) / 1024f, 2);
        }
    }
}
