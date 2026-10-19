---
id: GAME-010
title: "Stage summary, replay và quay lại World Map"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: gameplay
dependencies: [GAME-002, GAME-003, MAP-004]
source: "PRD Core Game Loop; reward"
stack: [react, phaser-3, dotnet, postgresql]
---

# GAME-010 — Stage summary, replay và quay lại World Map

> **Spec-driven ticket** · Domain: **Mini-games & Reward** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn biết mình nhận gì sau màn và dễ tiếp tục.

**Nguồn:** PRD Core Game Loop; reward. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Summary dùng dữ liệu completion response từ server: stars, EXP, food, nextAvailableStage.
- Nút Retry, Continue và World Map; không hiển thị accuracy tiêu cực.
- Chỉ hiện reward chính thức sau backend commit; offline retry có thông báo đang đồng bộ.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given stage complete
When backend returns rewards
Then summary hiển thị đúng values.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given stage-complete timeout
When retry same key
Then không bị thưởng lặp.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given Continue
When click
Then chuyển stage AVAILABLE phù hợp hoặc về map.
```

## 4. Contract & Data Impact

- React/Phaser Event `dragon:stage-completed` DTO matches stage-complete response.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Backend rejected result: không giả hiển thị successful reward.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Completion navigation, idempotency, reward display, no next stage.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Summary UI
- [ ] Retry/back nav
- [ ] Bridge refresh
- [ ] E2E.

## 8. Dependencies & Boundaries

**Blocked by:** [GAME-002](../gameplay/GAME-002-ghi-nhan-ket-qua-stage-idempotent-va-chong-gian-lan.md), [GAME-003](../gameplay/GAME-003-quy-tac-thuong-food-exp-stars.md), [MAP-004](../world-map/MAP-004-chon-stage-va-chuyen-phaser-mini-game.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt hành vi retry để ôn tập có nhận thêm thưởng không.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
