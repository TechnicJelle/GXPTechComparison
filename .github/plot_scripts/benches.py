import sys
import numpy as np
import matplotlib.pyplot as plt

dark = "dark" in sys.argv

plt.rcParams.update({
    "font.family": "Roboto",
    "axes.facecolor": "#FFFFFF11",
    "savefig.facecolor": "#00000000",
    "savefig.edgecolor": "#00000000",
})

if dark:
    plt.rcParams.update({
        "lines.color": "white",
        "text.color": "black",
        "axes.edgecolor": "lightgray",
        "axes.labelcolor": "white",
        "xtick.color": "white",
        "ytick.color": "white",
        "grid.color": "lightgray",
        "figure.facecolor": "black",
        "figure.edgecolor": "black",
    })

BenchSysDrawing = np.genfromtxt("in/BenchSysDrawing.ImageLoader-report.csv", dtype=None, delimiter=',', names=True, encoding=None)
BenchSkiaSharp = np.genfromtxt("in/BenchSkiaSharp.ImageLoader-report.csv", dtype=None, delimiter=',', names=True, encoding=None)

sdMean = float(str(BenchSysDrawing["Mean"]).strip("ms"))
sdStd = float(str(BenchSysDrawing["StdDev"]).strip("ms"))

ssMean = float(str(BenchSkiaSharp["Mean"]).strip("ms"))
ssStd = float(str(BenchSkiaSharp["StdDev"]).strip("ms"))

capsize=10
plt.bar(f"System.Drawing\n({sdMean} ms)", sdMean, yerr=sdStd, capsize=capsize, ecolor="gray")
plt.bar(f"SkiaSharp\n({ssMean} ms)", ssMean, yerr=ssStd, capsize=capsize, ecolor="gray")

plt.ylabel("ms to load image")

plt.tight_layout()
if dark:
    plt.savefig("out/benches_dark.svg")
else:
    plt.savefig("out/benches.svg")

