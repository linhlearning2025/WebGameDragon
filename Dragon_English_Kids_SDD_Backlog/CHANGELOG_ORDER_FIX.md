# Change log — corrected sequential implementation plan

- No tickets added or deleted: **58 IDs preserved**, **58 separate Markdown ticket files preserved**.
- Added strict 001–058 execution order (safe topological order with serial product-flow preference).
- **ASSET-003** now depends on `MAP-003` because dynamic state comes from backend map DTO.
- **MAP-004** now depends on `GAME-001` and moves **M1 → M2**; M1 delivers World Map display/demo without real play routing.
- **QA-003** now depends on `GUEST-006`, `GAME-010`, `DRAGON-002` for its full integrated test path; the report is deferred to M3.
- Rebuilt ticket index and roadmap; updated each changed ticket's frontmatter and dependency section.
- Added machine-readable sequence and progress metadata plus validator / next-ticket CLI.
- Milestone counts changed: M0 **7**, M1 **19**, M2 **23**, M3 **9**.
- Explicit integration/release gates cover content licensing, approved media, privacy, scoring/unlocks, speaking and stage integrity; they are not silently added as unconditional code dependencies.
- **Outstanding**: all tickets remain `DRAFT` until acceptance criteria and open decisions are approved.
