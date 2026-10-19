---
id: GUEST-001
title: "Khởi tạo phiên guest và chọn độ tuổi"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: guest
dependencies: [PLAT-003]
source: "PRD US-2.1; POST /guest/init"
stack: [react, phaser-3, dotnet, postgresql]
---

# GUEST-001 — Khởi tạo phiên guest và chọn độ tuổi

> **Spec-driven ticket** · Domain: **Guest Trial & Placement** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé lần đầu, tôi muốn chơi thử ngay mà không cần tạo tài khoản.

**Nguồn:** PRD US-2.1; POST /guest/init. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Chọn tuổi 5–8; server cấp guest_token ngẫu nhiên, TTL cấu hình.
- Không ghi dữ liệu nhận dạng cá nhân ngoài dữ liệu tối thiểu phục vụ trial.
- Rate-limit init, không cho client tự gán UUID/tier.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given tuổi 6
When init
Then nhận token và placement session hợp lệ.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given tuổi ngoài 5–8
When init
Then 400 validation.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given token hết hạn
When dùng
Then yêu cầu khởi tạo lại/được hướng dẫn rõ.
```

## 4. Contract & Data Impact

- `POST /api/v1/guest/init` `{selectedAge}` -> `{guestToken, expiresAt, firstQuestion}` (firstQuestion theo PRD).
- guest_sessions selected_age, temp_test_result JSONB, expires_at.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Refresh/tab close: chính sách giữ session trong storage phù hợp, không lưu token trong URL.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Age boundaries, token entropy, expiry, rate limit.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Guest API
- [ ] Age selection UI
- [ ] Persist/recovery
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [PLAT-003](../platform/PLAT-003-postgresql-schema-migrations-va-domain-constraints.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt TTL, nơi lưu token phía web, guest re-entry.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
