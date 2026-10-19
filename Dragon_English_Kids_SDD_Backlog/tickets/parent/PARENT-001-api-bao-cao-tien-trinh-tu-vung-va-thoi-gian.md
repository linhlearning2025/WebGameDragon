---
id: PARENT-001
title: "API báo cáo tiến trình từ vựng và thời gian"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P1
milestone: M3
domain: parent
dependencies: [AUTH-003, SRS-001, GAME-002]
source: "PRD US-1.4; GET parent report"
stack: [react, phaser-3, dotnet, postgresql]
---

# PARENT-001 — API báo cáo tiến trình từ vựng và thời gian

> **Spec-driven ticket** · Domain: **Parent Dashboard & Donation** · Priority: **P1** · Release: **M3** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là phụ huynh, tôi muốn xem số từ bé đã học, mastered và thời lượng chơi.

**Nguồn:** PRD US-1.4; GET parent report. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Tính unique vocab practiced, mastered theo threshold, due count, stage stars, playtime by date.
- Thời gian chơi dựa trusted session telemetry; không cộng khi tab hidden hoặc video paused.
- Aggregate theo child/time range, không đưa raw audio/transcript nếu không cần.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given child có các phiên
When GET report
Then metrics nhất quán với ledger.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given child chưa chơi
When GET
Then số liệu bằng 0, không 404.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given người không sở hữu child
When GET
Then denied.
```

## 4. Contract & Data Impact

- `GET /api/v1/parent/children/{id}/report?from=&to=`; DTO metrics with timezone boundaries.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Missing telemetry không giả định 0 là chắc chắn; flag data completeness.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Aggregation invariants, date boundaries, ownership, empty data.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Report query
- [ ] Session aggregation
- [ ] DTO/API
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [AUTH-003](../identity/AUTH-003-tao-chinh-sua-ho-so-tre-toi-a-5.md), [SRS-001](../srs/SRS-001-domain-srs-mastery-streak-va-lich-on.md), [GAME-002](../gameplay/GAME-002-ghi-nhan-ket-qua-stage-idempotent-va-chong-gian-lan.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Định nghĩa “đã thuộc” (mastery≥?); giới hạn thời gian UI và timezone gia đình.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
