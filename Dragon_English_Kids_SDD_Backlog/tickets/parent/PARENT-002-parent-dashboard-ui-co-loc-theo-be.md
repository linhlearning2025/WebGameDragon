---
id: PARENT-002
title: "Parent Dashboard UI có lọc theo bé"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P1
milestone: M3
domain: parent
dependencies: [PARENT-001, AUTH-005]
source: "PRD US-1.4; §1 React parent layer"
stack: [react, phaser-3, dotnet, postgresql]
---

# PARENT-002 — Parent Dashboard UI có lọc theo bé

> **Spec-driven ticket** · Domain: **Parent Dashboard & Donation** · Priority: **P1** · Release: **M3** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là phụ huynh, tôi muốn xem báo cáo dễ hiểu mà bé không tự mở được.

**Nguồn:** PRD US-1.4; §1 React parent layer. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Chỉ React render parent dashboard sau Parental Gate; chọn child/time range.
- Hiển thị practiced/mastered, due list, playtime và xu hướng cơ bản có giải thích.
- Không sử dụng đánh giá xếp hạng gây áp lực so sánh giữa các bé.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given gate đã verify
When mở dashboard
Then thấy dữ liệu child được chọn.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given gate expired
When truy cập lại
Then yêu cầu PIN.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given API empty
When render
Then empty state thân thiện.
```

## 4. Contract & Data Impact

- React route `/parent/reports`, report API.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Data stale: hiển thị last updated, retry.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Protected route E2E, empty/stale/loading, mobile layout.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Dashboard components
- [ ] Filters
- [ ] Gate integration
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [PARENT-001](../parent/PARENT-001-api-bao-cao-tien-trinh-tu-vung-va-thoi-gian.md), [AUTH-005](../identity/AUTH-005-parental-gate-voi-pin-bam-va-xac-thuc.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt KPI danh sách chính và thiết kế chart.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
