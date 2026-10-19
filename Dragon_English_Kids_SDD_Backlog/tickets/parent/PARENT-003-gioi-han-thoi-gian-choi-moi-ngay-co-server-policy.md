---
id: PARENT-003
title: "Giới hạn thời gian chơi mỗi ngày có server policy"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: parent
dependencies: [AUTH-003, PLAT-005]
source: "PRD US-1.3; users.daily_limit_mins default 20"
stack: [react, phaser-3, dotnet, postgresql]
---

# PARENT-003 — Giới hạn thời gian chơi mỗi ngày có server policy

> **Spec-driven ticket** · Domain: **Parent Dashboard & Donation** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là phụ huynh, tôi muốn giới hạn thời gian chơi mỗi ngày mà trẻ không tự bỏ qua.

**Nguồn:** PRD US-1.3; users.daily_limit_mins default 20. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Default 20 minutes/day theo PRD; setting thuộc parent, game engine dừng học khi đạt limit nhưng cho phép lưu completion.
- Track active session time server-assisted, pause khi hidden/idle theo thresholds.
- Mỗi lần đổi limit cần parental gate và audit.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given 20 phút dùng hết
When mở stage mới
Then backend không tạo stage session.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given đang ở câu cuối lúc hết giờ
When complete
Then hoàn tất an toàn theo grace period policy.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given phụ huynh thay 30 phút
When verify gate
Then policy áp dụng sau refresh.
```

## 4. Contract & Data Impact

- Proposed `/api/v1/parent/settings/playtime` PATCH; `/api/v1/game/{childId}/time-budget` GET; session ledger.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Client clock manipulation không cấp thêm giờ; timezone reset chốt bằng family timezone.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Clock tampering, midnight boundary, inactive tab, concurrent devices.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Policy engine
- [ ] Session accounting
- [ ] Parent settings
- [ ] E2E.

## 8. Dependencies & Boundaries

**Blocked by:** [AUTH-003](../identity/AUTH-003-tao-chinh-sua-ho-so-tre-toi-a-5.md), [PLAT-005](../platform/PLAT-005-kiem-soat-truy-cap-theo-phu-huynh-tre-va-khach.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- PRD đặt limit ở users, cần chốt tính cho mỗi bé hay cả gia đình; grace period, reset timezone.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
