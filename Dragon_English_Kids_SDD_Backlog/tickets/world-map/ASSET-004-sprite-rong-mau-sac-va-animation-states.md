---
id: ASSET-004
title: "Sprite Rồng màu sắc và animation states"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P1
milestone: M2
domain: world-map
dependencies: [ASSET-001, DRAGON-001]
source: "PRD US-3.1; folder dragon/baby, animations"
stack: [react, phaser-3, dotnet, postgresql]
---

# ASSET-004 — Sprite Rồng màu sắc và animation states

> **Spec-driven ticket** · Domain: **World Map & Assets** · Priority: **P1** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi nhìn thấy Rồng của mình di chuyển/ăn/vui tương ứng với trạng thái.

**Nguồn:** PRD US-3.1; folder dragon/baby, animations. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Map animation intents idle/walk/fly/eat/happy sang frame ranges versioned; không assume mọi sprite sheet có cùng frame count.
- Skin/color lấy từ dragon state; animation fallback idle nếu frame không đủ.
- Animation phải tạm dừng khi app không active, không tích lũy timer vô hạn.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given red dragon + mood happy
When lair load
Then dragon đúng palette và plays happy.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given feed success
When animation trigger
Then eat chạy một lần rồi trở lại idle.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given missing skin key
When render
Then dùng safe default và ghi warning.
```

## 4. Contract & Data Impact

- Dragon visual DTO `{colorId,evolutionLevel,skinIds,moodScore}`; sprite animation registry.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Reduced motion setting: static frames thay loop animation.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Animation state tests, missing frames, mobile memory budget.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Register animation atlas
- [ ] Implement FSM
- [ ] Fallback
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [ASSET-001](../world-map/ASSET-001-chuan-asset-manifest-va-preload-theo-phien-ban.md), [DRAGON-001](../dragon/DRAGON-001-hang-rong-render-state-va-ieu-huong.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Frame art là sample: cần QA cutout/art direction nếu dùng production.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
