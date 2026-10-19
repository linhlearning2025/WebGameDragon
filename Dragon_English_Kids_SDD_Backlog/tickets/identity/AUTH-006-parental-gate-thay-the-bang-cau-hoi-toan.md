---
id: AUTH-006
title: "Parental Gate thay thế bằng câu hỏi toán"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P2
milestone: M3
domain: identity
dependencies: [AUTH-005]
source: "PRD US-1.3"
stack: [react, phaser-3, dotnet, postgresql]
---

# AUTH-006 — Parental Gate thay thế bằng câu hỏi toán

> **Spec-driven ticket** · Domain: **Identity & Child Profiles** · Priority: **P2** · Release: **M3** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là phụ huynh, tôi có thể chọn phép toán thay thế hoặc bổ trợ PIN cho vùng quản trị.

**Nguồn:** PRD US-1.3. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Challenge sinh server-side có TTL, random và nonce; client không nhận đáp án.
- Challenge one-time, giới hạn thử sai, invalidate sau thành công.
- Phải có policy lựa chọn gate mode cho từng gia đình.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given challenge còn hạn
When trả lời đúng
Then có gate grant.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given replay challenge đã dùng
When submit
Then thất bại.
```

## 4. Contract & Data Impact

- Proposed `/api/v1/parent/gate/challenge`, `/gate/verify-math`.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Trẻ 5–8 tuổi có thể giải phép toán; mặc định PIN được ưu tiên cho settings nhạy cảm.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] TTL/replay/security tests.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Challenge API
- [ ] Gate UI
- [ ] Replay tests.

## 8. Dependencies & Boundaries

**Blocked by:** [AUTH-005](../identity/AUTH-005-parental-gate-voi-pin-bam-va-xac-thuc.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Đánh giá mức độ bảo vệ của math challenge trước khi sử dụng thay PIN cho thanh toán/xóa.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-02 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
