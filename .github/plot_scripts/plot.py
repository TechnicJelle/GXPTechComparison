import numpy as np
import matplotlib.pyplot as plt

gl = np.loadtxt("in/milliseconds_legacy.txt")
gm = np.loadtxt("in/milliseconds_modern.txt")

plt.figure(figsize=(7,4))

linewidth=0.1
plt.plot(gl, linewidth=linewidth, label="Legacy")
plt.plot(gm, linewidth=linewidth, label="Modern")

leg = plt.legend()
plt.xlabel("frame")
plt.ylabel("ms/frame")

# set the linewidth of each legend object
for legobj in leg.legendHandles:
    legobj.set_linewidth(2.0)

plt.tight_layout()
plt.savefig("out/plot.png", dpi=600)
