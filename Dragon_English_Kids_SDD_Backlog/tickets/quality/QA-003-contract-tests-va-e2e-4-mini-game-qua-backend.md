---
id: QA-003
title: "Contract tests và E2E 4 mini-game qua backend"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: quality
dependencies: [GAME-005, GAME-006, GAME-007, GAME-009, MAP-003, GUEST-006, GAME-010, DRAGON-002]
source: "PRD §5 endpoints; integration quality"
stack: [react, phaser-3, dotnet, postgresql]
---

# QA-003 — Contract tests và E2E 4 mini-game qua backend

> **Spec-driven ticket** · Domain: **Quality, Privacy & Release** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là đội dev, tôi muốn chắc chắn frontend/backend không lệch DTO và các flow trọng yếu chạy đúng.

**Nguồn:** PRD §5 endpoints; integration quality. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- OpenAPI schemas versioned, consumer contract tests cho map, stage-data, complete, dragon feed và auth.
- E2E M2 golden path: guest→placement→claim→map→4 skills→reward→lair/feed. Parent report (`PARENT-001/002`) được test bổ sung trong M3; không coi phần báo cáo là điều kiện hoàn thành M2.
- Negative paths: expired session, revoked access, offline storage, duplicate complete, mic denied.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given breaking API DTO change
When CI contract test
Then build fail.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given guest happy path
When chạy E2E
Then progress và dragon reward được giữ sau claim.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given double stage-complete
When test
Then ledger vẫn một reward.
```

## 4. Contract & Data Impact

- `contracts/openapi` snapshot, fixtures, seeded test DB, CI artifact screenshots.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- 3rd party AI/TTS không được gọi thật trong unit/E2E, dùng provider stubs.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Contract CI, E2E matrix, test data isolation.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] OpenAPI snapshots
- [ ] Test fixtures
- [ ] E2E flows
- [ ] CI gate.

## 8. Dependencies & Boundaries

**Blocked by:** [GAME-005](../gameplay/GAME-005-listening-rong-op-may-ung-hinh.md), [GAME-006](../gameplay/GAME-006-reading-chon-tu-pha-a.md), [GAME-007](../gameplay/GAME-007-spelling-ghep-phonics-xay-cau.md), [GAME-009](../gameplay/GAME-009-speaking-fuzzy-matching-va-2-strike-effort-pass.md), [MAP-003](../world-map/MAP-003-api-world-map-theo-profile-tier.md), [GUEST-006](../guest/GUEST-006-claim-guest-progress-vao-parent-account-mot-lan.md), [GAME-010](../gameplay/GAME-010-stage-summary-replay-va-quay-lai-world-map.md), [DRAGON-002](../dragon/DRAGON-002-cho-rong-an-tieu-hao-food-cong-exp.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chọn Playwright/Cypress và provider mocks tùy repo.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
