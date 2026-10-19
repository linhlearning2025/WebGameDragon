---
id: PLAT-004
title: "Correlation ID, logs, metrics và lỗi chuẩn"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P1
milestone: M0
domain: platform
dependencies: [PLAT-001]
source: "PRD §1 kiến trúc; yêu cầu vận hành bổ sung"
stack: [react, phaser-3, dotnet, postgresql]
---

# PLAT-004 — Correlation ID, logs, metrics và lỗi chuẩn

> **Spec-driven ticket** · Domain: **Foundation & Architecture** · Priority: **P1** · Release: **M0** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là dev/ops, tôi cần truy vết lỗi xuyên React, Phaser, API mà không ghi dữ liệu nhạy cảm của trẻ.

**Nguồn:** PRD §1 kiến trúc; yêu cầu vận hành bổ sung. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Mỗi HTTP request có correlation ID trả về response.
- Error response dùng ProblemDetails có `code`, `traceId` và status; không trả stack trace.
- Theo dõi API p95, lỗi stage, tải asset, mic availability, claim failure theo aggregate.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given API exception
When client nhận
Then có stable error code và traceId.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given nội dung log
When audit
Then không chứa tên trẻ, audio gốc, PIN, token hoặc câu trả lời raw.
```

## 4. Contract & Data Impact

- Header `X-Correlation-ID` (proposed), RFC 7807-compatible response.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Tracing exporter outage: request chính vẫn chạy.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Redaction unit tests, tracing smoke, problem mapping.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Thiết lập middleware
- [ ] Client error boundary
- [ ] Bổ sung dashboard/alerts tối thiểu.

## 8. Dependencies & Boundaries

**Blocked by:** [PLAT-001](../platform/PLAT-001-thiet-lap-monorepo-va-moi-truong-phat-trien.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chọn telemetry provider và retention.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-02 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
