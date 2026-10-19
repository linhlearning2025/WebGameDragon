---
id: AUTH-005
title: "Parental Gate với PIN băm và xác thực"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: identity
dependencies: [AUTH-001, PLAT-005]
source: "PRD US-1.3; POST /parent/verify-pin"
stack: [react, phaser-3, dotnet, postgresql]
---

# AUTH-005 — Parental Gate với PIN băm và xác thực

> **Spec-driven ticket** · Domain: **Identity & Child Profiles** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là phụ huynh, tôi muốn chặn trẻ mở phần quản trị bằng PIN.

**Nguồn:** PRD US-1.3; POST /parent/verify-pin. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- PIN 4 chữ số chỉ lưu hash có salt, không lưu `CHAR(4)` plaintext như mô tả sơ lược trong PRD.
- Cấp token parental-gate ngắn hạn, giới hạn phạm vi cho settings/report/donate.
- Giới hạn số lần nhập sai và trả thông báo phù hợp không lộ PIN.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given PIN đúng
When verify
Then nhận gate grant có expiry.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given PIN sai liên tiếp
When vượt ngưỡng
Then tạm khóa xác thực.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given gate grant hết hạn
When mở settings
Then cần xác thực lại.
```

## 4. Contract & Data Impact

- `POST /api/v1/parent/verify-pin` request `{pin}`; response `{verified, gateExpiresAt}` đề xuất.
- Đổi `users.parent_pin CHAR(4)` thành `parent_pin_hash`.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Không log raw PIN; reset PIN yêu cầu xác thực lại tài khoản.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Hash migration, expiry, brute-force, protected route tests.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Domain policy/PIN hash
- [ ] Endpoint & rate limiter
- [ ] React gate modal
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [AUTH-001](../identity/AUTH-001-ang-ky-ang-nhap-phu-huynh-bang-email.md), [PLAT-005](../platform/PLAT-005-kiem-soat-truy-cap-theo-phu-huynh-tre-va-khach.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Đây là sửa đổi bảo mật bắt buộc so với schema PRD; thời hạn grant cần chốt.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
