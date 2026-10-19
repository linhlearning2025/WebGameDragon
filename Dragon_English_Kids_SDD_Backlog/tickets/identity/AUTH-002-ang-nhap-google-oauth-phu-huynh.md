---
id: AUTH-002
title: "Đăng nhập Google OAuth phụ huynh"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P1
milestone: M2
domain: identity
dependencies: [AUTH-001]
source: "PRD US-1.1; §2 Account Claiming"
stack: [react, phaser-3, dotnet, postgresql]
---

# AUTH-002 — Đăng nhập Google OAuth phụ huynh

> **Spec-driven ticket** · Domain: **Identity & Child Profiles** · Priority: **P1** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là phụ huynh, tôi muốn đăng nhập Google mà không cần tạo thêm mật khẩu.

**Nguồn:** PRD US-1.1; §2 Account Claiming. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Sử dụng OAuth2/OIDC Authorization Code + PKCE; validate state, nonce, redirect allowlist.
- Liên kết provider subject với tài khoản hiện có theo quy tắc xác minh; không tự ý merge tài khoản không xác thực.
- Flow lỗi provider không làm mất guest_token trong tab hiện tại.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given Google trả verified identity hợp lệ
When callback
Then parent session được tạo.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given state không hợp lệ
When callback
Then request bị từ chối.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given login provider bị hủy
When quay về app
Then guest progress vẫn còn.
```

## 4. Contract & Data Impact

- Proposed `/api/v1/auth/google/start`, `/callback`; bảng external_logins(user_id, provider, subject).

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Email không verified: không liên kết tài khoản theo email.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Mock OAuth callback, CSRF/state test, account-link collision.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Backend OAuth handler
- [ ] React entry/callback
- [ ] Security tests.

## 8. Dependencies & Boundaries

**Blocked by:** [AUTH-001](../identity/AUTH-001-ang-ky-ang-nhap-phu-huynh-bang-email.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt provider auth library và quy tắc liên kết email đã đăng ký.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
