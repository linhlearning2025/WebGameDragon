---
id: MAP-004
title: "Chọn stage và chuyển Phaser mini-game"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: world-map
dependencies: [ASSET-003, MAP-003, PLAT-002]
source: "PRD §2 Core game loop"
stack: [react, phaser-3, dotnet, postgresql]
---

# MAP-004 — Chọn stage và chuyển Phaser mini-game

> **Spec-driven ticket** · Domain: **World Map & Assets** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn bấm nút Nghe/Đọc/Ghép/Nói để vào đúng mini-game đã mở khóa.

**Nguồn:** PRD §2 Core game loop. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Tap node phát stageId+skill, React kiểm tra profile/permission và gọi stage-data trước khi Scene start.
- UI loading và chống double tap; Back quay về map và giữ camera position.
- Không tin trạng thái mở khóa chỉ ở client; backend recheck tại stage-data.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given AVAILABLE listening stage
When tap
Then Listening scene load đúng data.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given locked stage do dữ liệu stale
When request stage-data bị từ chối
Then quay map và refresh state.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given user double tap
When loading
Then chỉ một stage start.
```

## 4. Contract & Data Impact

- Bridge `dragon:level-selected`; SceneRouter skill→Listening/Reading/Spelling/Speaking.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Stage-data 404/timeout: retry, không mất map state.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] E2E click route, denied stale unlock, double tap, back navigation.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Scene routing
- [ ] Loading state
- [ ] Bridge integration
- [ ] E2E.

## 8. Dependencies & Boundaries

**Blocked by:** [ASSET-003](../world-map/ASSET-003-rendering-4-node-ky-nang-va-trang-thai-hoan-thanh.md), [MAP-003](../world-map/MAP-003-api-world-map-theo-profile-tier.md), [PLAT-002](../platform/PLAT-002-event-bridge-react-phaser-co-kieu-du-lieu.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt preload strategy và stage session token (liên quan GAME-002).

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
