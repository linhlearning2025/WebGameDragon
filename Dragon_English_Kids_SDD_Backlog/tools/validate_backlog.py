#!/usr/bin/env python3
"""Validate the SDD backlog, metadata and sequential implementation order."""
import csv, json, pathlib, re, sys
root=pathlib.Path(__file__).resolve().parent.parent
meta=root/'metadata'
tickets=json.loads((meta/'tickets.json').read_text(encoding='utf8'))
order=json.loads((meta/'implementation_order.json').read_text(encoding='utf8'))
T={t['id']:t for t in tickets}
errors=[]
if len(T)!=58 or len(tickets)!=58: errors.append(f"Expected 58 unique tickets; got {len(tickets)} / {len(T)}")
if len(order)!=len(T): errors.append('Order/tickets count mismatch')
pos={o['id']:o['sequence'] for o in order}
if len(pos)!=len(order) or sorted(pos.values())!=list(range(1,len(order)+1)): errors.append('Duplicate or missing order sequences')
if set(T)!=set(pos): errors.append('Order/ticket ID set mismatch')
phase_ids={'M0':0,'M1':1,'M2':2,'M3':3}
last_phase=-1
for o in order:
 tid=o['id'];t=T.get(tid)
 if not t:continue
 file=root/t['path']
 if not file.is_file(): errors.append(f"Missing file {tid}: {file}");continue
 s=file.read_text(encoding='utf8')
 m=re.search(r'^dependencies: \[(.*?)\]$',s,re.M)
 fm_deps=[v.strip() for v in m.group(1).split(',') if v.strip()] if m else None
 if fm_deps!=t['deps']:errors.append(f"Frontmatter dependency mismatch: {tid}")
 m=re.search(r'^milestone: (M\d)$',s,re.M)
 if not m or m.group(1)!=t['phase']:errors.append(f"Frontmatter milestone mismatch: {tid}")
 if f"id: {tid}" not in s:errors.append(f"Frontmatter ID mismatch: {tid}")
 if o['dependencies']!=t['deps'] or o['file']!=t['path'] or o['milestone']!=t['phase'] or o['priority']!=t['priority']:
  errors.append(f"Order metadata mismatch: {tid}")
 phase=phase_ids.get(t['phase'],-1)
 if phase<last_phase:errors.append(f"Milestone order reversal at {tid}")
 last_phase=phase
 for dep in t['deps']:
  if dep not in T:errors.append(f"Unknown dependency {tid} <- {dep}")
  elif pos.get(dep,10**9)>=pos.get(tid,0):errors.append(f"Invalid dependency order {tid} <- {dep}")
index=(root/'TICKET_INDEX.md').read_text(encoding='utf8')
for o in order:
 if f"| **{o['sequence']:03}** | [{o['id']} — " not in index: errors.append(f"Missing index entry: {o['id']}")
files=list((root/'tickets').rglob('*.md'))
if len(files)!=len(T):errors.append(f"Ticket Markdown count mismatch: {len(files)}")
with (meta/'tickets.csv').open(encoding='utf-8-sig',newline='') as f: rows=list(csv.DictReader(f))
if [r['id'] for r in rows]!=[o['id'] for o in order]:errors.append('tickets.csv order mismatch')
with (meta/'implementation_order.csv').open(encoding='utf-8-sig',newline='') as f: rows2=list(csv.DictReader(f))
if [r['ticket_id'] for r in rows2]!=[o['id'] for o in order]:errors.append('implementation_order.csv order mismatch')
progress=json.loads((meta/'progress.json').read_text(encoding='utf8'))
if set(progress)!=set(T):errors.append('progress.json missing/unrecognized tickets')
for tid,info in progress.items():
 if info.get('status') not in {'TODO','IN_PROGRESS','BLOCKED','DONE'}:errors.append(f"Invalid status for {tid}")
print(f"Validating {len(T)} tickets, {len(files)} markdown files, {len(order)} sequence entries")
if errors:
 for e in errors:print('ERROR:',e)
 sys.exit(1)
print('PASS: 58/58, dependency/phase/order/frontmatter/index/CSV/progress consistent')
