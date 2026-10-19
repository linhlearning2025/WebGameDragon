---
id: SRS-002
title: "Query từ đến hạn và phân phối pool theo tier"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: srs
dependencies: [SRS-001, CONTENT-002]
source: "PRD US-4.2"
stack: [react, phaser-3, dotnet, postgresql]
---

# SRS-002 — Query từ đến hạn và phân phối pool theo tier

> **Spec-driven ticket** · Domain: **Spaced Repetition** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là stage selection engine, tôi cần danh sách due vocab đúng ngữ cảnh và ưu tiên hợp lý.

**Nguồn:** PRD US-4.2. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Query only reviewed due (`next_review_at <= nowUTC`) cho child/tier/eligible topic policy.
- Ưu tiên overdue, mastery thấp, tránh xuất hiện lại trong cùng stage và chống starvation.
- Không dùng vocab unpublished hay khác tier khi policy không cho phép.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given 3 vocab đến hạn
When query
Then trả ordered distinct IDs.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given từ archived
When due
Then không được chọn.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given 2 child
When query
Then mỗi người nhận pool độc lập.
```

## 4. Contract & Data Impact

- `GET` internal repository `GetDueVocab(childId,tier,topics,asOf)`; indexed child_vocab_srs.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Đến hạn nhiều hơn quota: phần còn lại giữ trong due pool, không xóa lịch.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Priority ordering, child isolation, query performance, policy scopes.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Repository query
- [ ] Priority algorithm
- [ ] Index/profiling
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [SRS-001](../srs/SRS-001-domain-srs-mastery-streak-va-lich-on.md), [CONTENT-002](../content/CONTENT-002-bo-du-lieu-chu-e-starters-movers-va-kiem-soat-trung.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt ưu tiên topic-local hay global review khi 30% SRS.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
