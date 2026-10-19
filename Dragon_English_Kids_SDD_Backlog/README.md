# Dragon English Kids — Spec-Driven Development Ticket Pack

**Version:** 1.0 (DRAFT) · **Created:** 2026-10-19 · **Language:** Vietnamese · **Source:** Dragon English Kids PRD supplied in conversation

## Bạn nhận được gì?

- **58 implementation tickets**, **mỗi ticket một file `.md`** trong `tickets/<domain>/`.
- Spec-driven format: **Business Context (WHY) → Functional Specification (WHAT) → Given/When/Then → Contracts → Errors → Verification → Tasks (HOW) → Dependencies → Open Decisions → DoD**.
- Phân chia **10 domain**, **4 milestones M0–M3**, dependency check và source traceability.
- Các file hỗ trợ: project constitution, domain map, roadmap, open decisions, ticket index, API index, sample contracts và ticket template.
- Tất cả ticket có `status: DRAFT`; **không thay PO/technical/legal approvals**.

## Bắt đầu theo SDD

1. Đọc `CONSTITUTION.md` và `DOMAIN_MAP.md`; duyệt với PO/architect.
2. Giải quyết quyết định bắt buộc trong `OPEN_DECISIONS.md`, nhất là rule unlock, reward, SRS và speaking privacy.
3. Duyệt M0/M1 P0 trong `TICKET_INDEX.md`, lấy spec đúng domain; không implement ticket có dependency chưa xong.
4. Với mỗi ticket: dùng **Specify → Clarify → Plan → Tasks → Implement → Analyze/Converge** trong workflow SDD đã chọn. Ticket này cung cấp WHAT, AC, contract và tasks gợi ý; bước Plan phải thêm thiết kế kỹ thuật cụ thể cho repo thật.
5. Gắn ticket ID vào branch, commit, PR và automated test case; kiểm thử Given/When/Then trước khi close.
6. `DELIVERY_ROADMAP.md` có thứ tự đề xuất theo dependency; `REQUIREMENT_TRACEABILITY.md` đối chiếu về PRD.

## Cấu trúc thư mục

```text
Dragon_English_Kids_SDD_Backlog/
├── README.md
├── CONSTITUTION.md
├── DOMAIN_MAP.md
├── DELIVERY_ROADMAP.md
├── TICKET_INDEX.md
├── REQUIREMENT_TRACEABILITY.md
├── OPEN_DECISIONS.md
├── API_CONTRACT_INDEX.md
├── TICKET_TEMPLATE.md
├── tickets/
│   ├── platform/
│   ├── identity/
│   ├── guest/
│   ├── content/
│   ├── world-map/
│   ├── dragon/
│   ├── srs/
│   ├── gameplay/
│   ├── parent/
│   └── quality/
├── contracts/
│   ├── world-map.sample.json
│   ├── stage-complete.sample.json
│   └── srs-policy.proposed.json
└── metadata/
    ├── tickets.json
    ├── tickets.csv
    └── validation_report.json
```

## Scope & non-scope của bundle

**Covers:** yêu cầu trong PRD account/guest/placement/claim, bốn mini-games, world map/assets, dragon meta-game, SRS, AI/TTS/R2, Parent Dashboard, donate, privacy and production readiness.

**Does not include:** implementation source code, approved OpenAPI production schema, actual Cambridge-licensed content dataset, final 5-stage dragon evolution art, sprint estimation/owners, legally verified child privacy policy, or final acceptance sign-off. Bản mẫu DTO trong `contracts/` là **proposed**, không nên copy làm contract production mà không review.

## Naming & ticket policy

- `P0`: blocker/critical path; `P1`: high priority; `P2`: later phase.
- `M0`: foundation; `M1`: guest-to-map MVP; `M2`: learning core; `M3`: parent/launch.
- Ticket IDs ổn định (`GAME-009`…), mỗi ticket một file và có link dependencies.
- Chỉ các API xuất hiện trong PRD mới được ghi là existing; API mới được đánh dấu proposed.
- Estimated SP/timeline để `TBD` theo planning với team; tránh bịa estimates.

## Standards/reference

Cấu trúc gợi ý theo triết lý [GitHub Spec Kit — Spec-Driven Development](https://github.com/github/spec-kit) nhưng không giả định đây là template chính thức bắt buộc hay project đã cài Spec Kit. Bản PRD hiện tại dùng domain-oriented decomposition để mỗi feature có thể triển khai độc lập.
