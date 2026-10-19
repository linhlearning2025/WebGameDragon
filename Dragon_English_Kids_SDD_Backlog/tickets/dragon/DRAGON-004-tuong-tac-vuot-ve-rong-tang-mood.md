---
id: DRAGON-004
title: "Tương tác vuốt ve Rồng tăng Mood"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P2
milestone: M3
domain: dragon
dependencies: [DRAGON-001]
source: "PRD US-3.1 mood_score"
stack: [react, phaser-3, dotnet, postgresql]
---

# DRAGON-004 — Tương tác vuốt ve Rồng tăng Mood

> **Spec-driven ticket** · Domain: **Dragon Lair & Evolution** · Priority: **P2** · Release: **M3** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn vuốt ve Rồng để Rồng vui vẻ.

**Nguồn:** PRD US-3.1 mood_score. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Touch/click gesture hợp lệ phát animation happy, tăng Mood theo cooldown server-side.
- Mood score có min/max, giảm theo thời gian nếu game design chốt; không thưởng vô hạn bằng spam.
- Không được yêu cầu micro/camera cho tính năng này.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given pet hợp lệ
When gửi
Then mood tăng trong range.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given spam nhiều lần trước cooldown
When gửi
Then server không cộng thêm.
```

## 4. Contract & Data Impact

- Proposed `POST /api/v1/game/{childId}/dragon/pet` response mood/cooldown.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Tab background: không xử lý gesture ảo.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Touch gesture, throttle, bounds and reconnect.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Gesture handler
- [ ] Mood policy
- [ ] Feedback animation
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [DRAGON-001](../dragon/DRAGON-001-hang-rong-render-state-va-ieu-huong.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Mood decay/cooldown reward tuning cần PO quyết định.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-02 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
