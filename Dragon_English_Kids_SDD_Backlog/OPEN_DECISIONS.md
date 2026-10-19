# OPEN_DECISIONS — Business/Architecture clarification log

> Each proposed value below is **NOT approved**. BA/PO/technical lead should confirm before implementing the related ticket. Suggested decision owner is indicative.

| ID | Decision | Proposal / risk | Affected tickets | Owner |
|---|---|---|---|---|
| DEC-01 | Placement thresholds: STARTERS/MOVERS | Define age baseline, streak escalation/de-escalation, 6–8 Q scoring and manual override | GUEST-003/004 | Learning designer + PO |
| DEC-02 | Stage/Island catalog | Specify real islands/topics, total stage count, dependency graph and mapping to 4 skills | MAP-001/002, CONTENT-002 | PO + Content |
| DEC-03 | Star scoring | Define 0–3 stars accuracy/effort rules and unlock thresholds | GAME-003, MAP-002 | PO |
| DEC-04 | Session length / 70:30 rounding | 10 questions => 7 new+3 review when pools sufficient; choose rounding and shortage fallback | GAME-001, SRS-002 | PO + Learning |
| DEC-05 | SRS schedule | Choose actual intervals, mastery transitions, wrong/effort pass treatment | SRS-001 | Learning designer |
| DEC-06 | Speaking evaluation | Select browser/device support, transcript trust, fuzzy thresholds, mic permissions and fallback | GAME-008/009, QA-002 | Privacy + Learning + FE |
| DEC-07 | Daily playtime scope | Limit per family vs child, reset timezone, idle/pause and grace period | PARENT-003 | PO + Parent advisor |
| DEC-08 | Guest lifetime | TTL, re-entry after expired, retention, 5-child claim limits | GUEST-001/006 | PO + Security |
| DEC-09 | Auth sessions | Cookies/JWT, verified email policy, reset password, OAuth linking | AUTH-001/002 | Security + Backend |
| DEC-10 | Parental Gate | PIN hash storage, grant duration, math-only gate risk | AUTH-005/006 | Security + PO |
| DEC-11 | Food & Dragon XP | food reward conversion, XP thresholds for 5 levels, evolution assets | GAME-003, DRAGON-002/003 | Game designer |
| DEC-12 | Content licensing | Cambridge wordlist version, images/audio generation rights and provider terms | CONTENT-002/003/004 | Content + Legal |
| DEC-13 | Donation verification | Manual confirmation vs webhook, supporter scope/revocation | PARENT-004/005 | PO + Finance |
| DEC-14 | Map renderer assets | Production PNG cutouts/atlases vs existing contact-sheet previews; world coords/art sizes | ASSET-001/002/004 | Game artist + FE |
| DEC-15 | Privacy jurisdiction | Country distribution, child privacy notice, parent consent, data deletion and speech vendor | QA-002, AUTH-007 | Legal + Security |
| DEC-16 | Performance SLA | Cached-audio <100ms benchmark, devices, network and percentile | QA-004 | FE + QA |
| DEC-17 | Stage session integrity | Validate answers server-side or signed attempt protocol; how to handle offline completion | GAME-002 | Backend + Security |

## BA clarification checklist

- [ ] Review domain boundaries and product principles.
- [ ] Approve AC and proposed API contracts for M0/M1 P0 tickets first.
- [ ] Resolve P0 decisions DEC-01,02,03,04,05,06,07,08,10,14,17.
- [ ] Approve privacy and parental consent controls before production pilot.
- [ ] Attach test evidence and sign-off to each ticket before closing it.
