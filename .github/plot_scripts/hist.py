import sys
import numpy as np
import matplotlib.pyplot as plt

dark = "dark" in sys.argv

plt.rcParams.update({
    "font.family": "Roboto",
    "axes.facecolor": "#00000000",
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

gl = np.loadtxt("in/milliseconds_legacy.txt")
gm = np.loadtxt("in/milliseconds_modern.txt")

avgl = np.average(gl)
avgm = np.average(gm)

plt.xlim(0.25, 0.6)

bins = int(max(len(gl), len(gm)) / 10)
plt.hist(gl, bins=bins, label="Legacy " + f"(avg: {avgl:.2f} ms)")
plt.hist(gm, bins=bins, label="Modern " + f"(avg: {avgm:.2f} ms)")

legend = plt.legend(labelcolor = "linecolor")
legend.get_frame().set_alpha(None)
plt.xlabel("ms/frame")
plt.ylabel("occurrences")

plt.tight_layout()
if dark:
    plt.savefig("out/hist_dark.svg")
else:
    plt.savefig("out/hist.svg")
