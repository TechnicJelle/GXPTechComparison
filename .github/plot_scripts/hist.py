import numpy as np
import matplotlib.pyplot as plt

plt.rcParams["font.family"] = "Roboto"

gl = np.loadtxt("in/milliseconds_legacy.txt")
gm = np.loadtxt("in/milliseconds_modern.txt")

avgl = np.average(gl)
avgm = np.average(gm)

plt.xlim(0.25, 0.6)

bins = int(max(len(gl), len(gm)) / 10)
plt.hist(gl, bins=bins, label="Legacy " + f"(avg: {avgl:.2f})")
plt.hist(gm, bins=bins, label="Modern " + f"(avg: {avgm:.2f})")

plt.legend()
plt.xlabel("ms/frame")
plt.ylabel("occurrences")

plt.tight_layout()
plt.savefig("out/hist.png", dpi=600)
