---
id: AUTH-001
title: "Đăng ký/đăng nhập phụ huynh bằng email"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: identity
dependencies: [PLAT-003, PLAT-005]
source: "PRD US-1.1; §3.1"
stack: [react, phaser-3, dotnet, postgresql]
---

# AUTH-001 — Đăng ký/đăng nhập phụ huynh bằng email

> **Spec-driven ticket** · Domain: **Identity & Child Profiles** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là phụ huynh, tôi muốn có tài khoản email để lưu và quản lý hồ sơ các bé.

**Nguồn:** PRD US-1.1; §3.1. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Email normalize và unique; mật khẩu lưu bằng adaptive password hashing.
- Đăng nhập thiết lập session bảo mật; thông báo lỗi không tiết lộ email tồn tại.
- Có validate input, rate-limit, lockout tạm thời chống brute force.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given email chưa dùng và dữ liệu hợp lệ
When đăng ký
Then tài khoản được tạo và có thể đăng nhập.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given email đã tồn tại
When đăng ký
Then nhận conflict/hướng dẫn an toàn không lộ dữ liệu khác.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given mật khẩu sai lặp lại
When vượt ngưỡng cấu hình
Then request bị hạn chế.
```

## 4. Contract & Data Impact

- Proposed `POST /api/v1/auth/register`, `/login`, `/logout`, `GET /me`.
- User table PRD; auth sessions table bổ sung.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Session hết hạn: điều hướng về parent login mà không xóa tiến trình guest chưa claim.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Auth integration, rate limit, session rotation, hashing verification.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] API auth
- [ ] Form React
- [ ] Email validation
- [ ] Security tests.

## 8. Dependencies & Boundaries

**Blocked by:** [PLAT-003](../platform/PLAT-003-postgresql-schema-migrations-va-domain-constraints.md), [PLAT-005](../platform/PLAT-005-kiem-soat-truy-cap-theo-phu-huynh-tre-va-khach.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- PRD chưa quyết định xác thực email, quên mật khẩu, session strategy; cần quyết định trước production.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
