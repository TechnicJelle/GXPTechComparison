mkdir -Force ./out/ | Out-Null
python benches.py
python benches.py dark
python hist.py
python hist.py dark
python plot.py
python plot.py dark
