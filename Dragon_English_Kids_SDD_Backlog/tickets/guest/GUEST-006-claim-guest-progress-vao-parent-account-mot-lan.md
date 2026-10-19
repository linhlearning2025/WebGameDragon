---
id: GUEST-006
title: "Claim guest progress vào parent account một lần"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: guest
dependencies: [GUEST-005, AUTH-001, AUTH-003]
source: "PRD US-2.4; `/auth/register-and-claim`"
stack: [react, phaser-3, dotnet, postgresql]
---

# GUEST-006 — Claim guest progress vào parent account một lần

> **Spec-driven ticket** · Domain: **Guest Trial & Placement** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là phụ huynh, tôi muốn đăng ký và giữ nguyên Rồng cùng kết quả trial của bé.

**Nguồn:** PRD US-2.4; `/auth/register-and-claim`. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Claim là transaction atomic: tạo/link user (nếu cần), child profile, dragon, placement history.
- Guest token được consume một lần, retry cùng operation trả kết quả cũ nếu owner trùng.
- Chặn claim token expired, consumed bởi người khác hoặc parent vượt max 5 profiles.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given guest hợp lệ và parent mới
When register-and-claim
Then child+dragon được tạo, guest token consumed.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given response timeout
When client retry cùng idempotency key
Then không có child duplicate.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given parent đã có 5 children
When claim
Then từ chối rõ và guest không bị consume.
```

## 4. Contract & Data Impact

- `POST /api/v1/auth/register-and-claim` `{guestToken, parentRegistration, childNickname, idempotencyKey}` proposed.
- DB transaction + claim audit record/unique guest_token.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Rollback nếu bất kỳ insert nào lỗi; không mất guest progress.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Concurrent claim collision, rollback, idempotent replay, ownership.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Claim service
- [ ] Transactional repository
- [ ] React register flow
- [ ] Race tests.

## 8. Dependencies & Boundaries

**Blocked by:** [GUEST-005](../guest/GUEST-005-no-trung-tao-rong-guest-va-tuy-chinh.md), [AUTH-001](../identity/AUTH-001-ang-ky-ang-nhap-phu-huynh-bang-email.md), [AUTH-003](../identity/AUTH-003-tao-chinh-sua-ho-so-tre-toi-a-5.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt claim khi parent đã đăng nhập dùng endpoint riêng hay cùng endpoint.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
