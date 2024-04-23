import numpy as np
import matplotlib.pyplot as plt

BenchSysDrawing = np.genfromtxt("in/BenchSysDrawing.ImageLoader-report.csv", dtype=None, delimiter=',', names=True, encoding=None)
BenchSkiaSharp = np.genfromtxt("in/BenchSkiaSharp.ImageLoader-report.csv", dtype=None, delimiter=',', names=True, encoding=None)

sdMean = float(str(BenchSysDrawing["Mean"]).strip("ms"))
sdStd = float(str(BenchSysDrawing["StdDev"]).strip("ms"))

ssMean = float(str(BenchSkiaSharp["Mean"]).strip("ms"))
ssStd = float(str(BenchSkiaSharp["StdDev"]).strip("ms"))

capsize=5
plt.bar("System.Drawing", sdMean, yerr=sdStd, capsize=capsize)
plt.bar("SkiaSharp", ssMean, yerr=ssStd, capsize=capsize)

plt.ylabel("ms to load image")

plt.tight_layout()
plt.savefig("out/benches.png", dpi=600)
