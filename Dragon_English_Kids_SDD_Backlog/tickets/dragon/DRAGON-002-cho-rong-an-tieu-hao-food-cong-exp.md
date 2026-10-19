---
id: DRAGON-002
title: "Cho Rồng ăn: tiêu hao food, cộng EXP"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: dragon
dependencies: [DRAGON-001, GAME-003]
source: "PRD POST `/game/{childId}/dragon/feed`"
stack: [react, phaser-3, dotnet, postgresql]
---

# DRAGON-002 — Cho Rồng ăn: tiêu hao food, cộng EXP

> **Spec-driven ticket** · Domain: **Dragon Lair & Evolution** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn dùng thức ăn thắng từ màn học để tăng EXP cho Rồng.

**Nguồn:** PRD POST `/game/{childId}/dragon/feed`. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Backend validate foodType/quantity, đủ tồn kho, cập nhật inventory và EXP trong một transaction.
- Mỗi feed request có idempotency key để tránh double spend từ double tap/retry.
- Response trả state mới và event thành công; client chỉ animate khi server commit.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given có 2 apples
When feed 1 apple
Then còn 1 và EXP tăng đúng config.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given không đủ apples
When feed
Then từ chối không đổi inventory/EXP.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given gửi lại same key
When server xử lý
Then không trừ thức ăn thêm lần hai.
```

## 4. Contract & Data Impact

- `POST /api/v1/game/{childId}/dragon/feed` `{foodType,quantity,idempotencyKey}`; reward config catalog.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Concurrent feed: row lock/version check; không cho negative inventory.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Atomicity, concurrency, replay, unauthorized profile.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Feed command handler
- [ ] Inventory transactions
- [ ] UI feedback
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [DRAGON-001](../dragon/DRAGON-001-hang-rong-render-state-va-ieu-huong.md), [GAME-003](../gameplay/GAME-003-quy-tac-thuong-food-exp-stars.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt food→EXP conversion và anti-spam limits.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
