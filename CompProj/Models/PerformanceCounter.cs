using System;
using System.Diagnostics;

namespace CompProj.Models {
    public class PerformanceCounter {
        private Stopwatch Watch = new Stopwatch();
        private double StartMemory { get; set; }
        public double UsedMemory { get; private set; }
        public long ElapsedTimeMs { get; private set; }

        public void Start() {
            Watch.Reset();
            Watch.Start();
            StartMemory = ConvertBytesToMegabytes(GC.GetTotalMemory(true));
        }

        public void Stop() {
            Watch.Stop();
            ElapsedTimeMs = Watch.ElapsedMilliseconds;
            UsedMemory = ConvertBytesToMegabytes(GC.GetTotalMemory(true));
        }

        public double ConvertBytesToMegabytes(long bytes) {
            return Math.Round((bytes / 1024f) / 1024f, 2);
        }
    }
}
