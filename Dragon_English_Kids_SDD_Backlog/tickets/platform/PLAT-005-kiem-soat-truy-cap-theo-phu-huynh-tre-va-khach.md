---
id: PLAT-005
title: "Kiểm soát truy cập theo phụ huynh, trẻ và khách"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M0
domain: platform
dependencies: [PLAT-003]
source: "PRD §2; §5 API"
stack: [react, phaser-3, dotnet, postgresql]
---

# PLAT-005 — Kiểm soát truy cập theo phụ huynh, trẻ và khách

> **Spec-driven ticket** · Domain: **Foundation & Architecture** · Priority: **P0** · Release: **M0** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là hệ thống, tôi phải chặn truy cập chéo dữ liệu trẻ và giới hạn chức năng guest.

**Nguồn:** PRD §2; §5 API. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Mọi `/game/{childId}` và `/parent/children/{id}` xác thực quyền sở hữu child profile phía server.
- Guest token chỉ được dùng trong flow thử và claim, không truy cập báo cáo phụ huynh.
- Hết hạn/revoked token trả 401; ownership mismatch trả 403/404 theo policy chống enumeration.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given parent A yêu cầu child của parent B
When gọi API
Then không có dữ liệu trả về.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given expired guest token
When submit test
Then từ chối không tạo child.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given thiếu bearer/cookie theo cơ chế đã chốt
When gọi protected API
Then 401.
```

## 4. Contract & Data Impact

- Auth policy `ParentOwnsChild`, `ValidGuest`; scope per endpoint.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Không tin `childId` được truyền từ client; kiểm tra DB trên từng request.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] IDOR tests, expired session, role matrix tests.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Định nghĩa policies
- [ ] Implement authorization handlers
- [ ] Test IDOR và permission.

## 8. Dependencies & Boundaries

**Blocked by:** [PLAT-003](../platform/PLAT-003-postgresql-schema-migrations-va-domain-constraints.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt cookie session vs JWT, CSRF và thời hạn token.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
