---
id: QA-005
title: "Phát hành, backup/restore, feature flags và runbook"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P1
milestone: M3
domain: quality
dependencies: [PLAT-004, QA-003]
source: "PRD triển khai production; vận hành bổ sung"
stack: [react, phaser-3, dotnet, postgresql]
---

# QA-005 — Phát hành, backup/restore, feature flags và runbook

> **Spec-driven ticket** · Domain: **Quality, Privacy & Release** · Priority: **P1** · Release: **M3** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là ops, tôi cần phát hành an toàn và xử lý sự cố không làm mất dữ liệu trẻ.

**Nguồn:** PRD triển khai production; vận hành bổ sung. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Deployment env dev/staging/prod, DB migration gate, secrets vault, R2 bucket rights và CORS.
- Backup PostgreSQL, kiểm thử restore theo chu kỳ; feature flag speaking/provider fallbacks.
- Runbook incident: lỗi CDN/TTS, API unavailable, database recovery, rollback content version.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given staging deployment
When smoke tests
Then auth/map/guest/stage endpoints healthy.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given restore rehearsal
When restore backup
Then kiểm tra referential integrity và dữ liệu mẫu.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given speaking issue
When flag off
Then UI fallback an toàn không cần deploy lại.
```

## 4. Contract & Data Impact

- CI/CD pipeline, recovery runbook, data retention/backups policy.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Migration breaking schema phải qua expand-and-contract thay vì xóa cột tức thì.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Deployment smoke, restore drill, flag toggle, secret audit.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] CI/CD deploy
- [ ] Backup/restore
- [ ] Feature flags
- [ ] Runbook.

## 8. Dependencies & Boundaries

**Blocked by:** [PLAT-004](../platform/PLAT-004-correlation-id-logs-metrics-va-loi-chuan.md), [QA-003](../quality/QA-003-contract-tests-va-e2e-4-mini-game-qua-backend.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt hosting, RPO/RTO và supported browsers với team.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
