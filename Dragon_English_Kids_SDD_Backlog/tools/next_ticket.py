#!/usr/bin/env python3
"""Print next sequential TODO ticket whose hard dependencies are DONE."""
import json,pathlib
root=pathlib.Path(__file__).resolve().parent.parent
order=json.loads((root/'metadata'/'implementation_order.json').read_text(encoding='utf8'))
progress=json.loads((root/'metadata'/'progress.json').read_text(encoding='utf8'))
def state(id):return progress.get(id,{}).get('status','TODO')
ready=[t for t in order if state(t['id'])=='TODO' and all(state(d)=='DONE' for d in t['dependencies'])]
in_progress=[t for t in order if state(t['id'])=='IN_PROGRESS']
if in_progress:
 print('IN_PROGRESS (finish before starting another if working serially):')
 for t in in_progress:print(f"  {t['sequence']:03}. {t['id']} — {t['title']}")
if ready:
 first=ready[0];print(f"NEXT READY: {first['sequence']:03} {first['id']} — {first['title']}")
 print('Markdown:',first['file']);print('Milestone:',first['milestone'],'Priority:',first['priority'])
 print('NOTE: Verify the BA/PO decisions in OPEN_DECISIONS.md before starting.')
 if len(ready)>1:
  print('Other technically READY (parallel work):',', '.join(t['id'] for t in ready[1:]))
else:
 print('No TODO ticket is technically READY. Check progress / BLOCKED dependencies.')
left=[t for t in order if state(t['id'])!='DONE']
print(f"Progress: {len(order)-len(left)}/{len(order)} DONE")
