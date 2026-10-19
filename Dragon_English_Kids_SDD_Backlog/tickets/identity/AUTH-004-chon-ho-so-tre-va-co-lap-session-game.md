---
id: AUTH-004
title: "Chọn hồ sơ trẻ và cô lập session game"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: identity
dependencies: [AUTH-003, PLAT-002]
source: "PRD §1 Profile Switcher; US-1.2"
stack: [react, phaser-3, dotnet, postgresql]
---

# AUTH-004 — Chọn hồ sơ trẻ và cô lập session game

> **Spec-driven ticket** · Domain: **Identity & Child Profiles** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là gia đình nhiều bé, tôi muốn đổi hồ sơ mà không trộn tiến trình.

**Nguồn:** PRD §1 Profile Switcher; US-1.2. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- React lưu selectedChildId trong state/session được kiểm soát, Phaser reload progress khi đổi.
- Chặn đổi profile trong lúc ghi stage-complete hoặc yêu cầu xác nhận rời màn.
- Mỗi request mang childId và server xác thực ownership.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given đổi từ bé A sang B
When map render
Then chỉ có level/progress/dragon của B.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given stage A đang chơi
When parent đổi B
Then stage A được thoát/lưu theo policy trước khi chuyển.
```

## 4. Contract & Data Impact

- Events `dragon:profile-changed` và route `/play/:childId` (đề xuất).

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- No selected child: mở profile switcher, không tự gán profile đầu tiên.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Cross-profile leak, navigation race, fresh session restoration.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Selector React
- [ ] Bridge flow
- [ ] Isolation tests.

## 8. Dependencies & Boundaries

**Blocked by:** [AUTH-003](../identity/AUTH-003-tao-chinh-sua-ho-so-tre-toi-a-5.md), [PLAT-002](../platform/PLAT-002-event-bridge-react-phaser-co-kieu-du-lieu.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt cơ chế auto-select cho gia đình một bé.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-02 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
