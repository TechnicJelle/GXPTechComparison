import numpy as np
import matplotlib.pyplot as plt

gl = np.loadtxt("in/milliseconds_legacy.txt")
gm = np.loadtxt("in/milliseconds_modern.txt")

plt.xlim(0.25, 0.6)

bins = int(max(len(gl), len(gm)) / 10)
plt.hist(gl, bins=bins, label="Legacy")
plt.hist(gm, bins=bins, label="Modern")

plt.legend()
plt.xlabel("ms/frame")
plt.ylabel("occurrences")

plt.tight_layout()
plt.savefig("out/hist.png", dpi=200)
