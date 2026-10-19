---
id: SRS-001
title: "Domain SRS mastery, streak và lịch ôn"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: srs
dependencies: [CONTENT-001, AUTH-003]
source: "PRD §3.6; 70/30 SRS"
stack: [react, phaser-3, dotnet, postgresql]
---

# SRS-001 — Domain SRS mastery, streak và lịch ôn

> **Spec-driven ticket** · Domain: **Spaced Repetition** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn từ sai được nhắc lại và từ đúng được ôn cách quãng.

**Nguồn:** PRD §3.6; 70/30 SRS. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Định nghĩa mastery 0–5, correctStreak, wrongCount, nextReviewAt; update per child+vocab.
- Mỗi evaluated attempt có outcome correct/wrong/effort-pass/unavailable, đúng đủ điều kiện mới tăng mastery.
- Rule versioned và deterministic; xử lý timezone bằng UTC timestamp.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given first correct attempt
When update
Then mastery/streak/nextReviewAt theo table rules.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given wrong answer
When update
Then reset/adjust streak, tăng wrong count và lên lịch ôn sớm hơn.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given effort-pass
When update
Then không tăng mastery giống correct.
```

## 4. Contract & Data Impact

- `child_vocab_srs` composite PK; proposed table SRS interval mapping `{mastery,intervalDays}` with version.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Duplicate attemptId: update đúng một lần; out-of-order events theo completion time policy.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] State transition table, UTC boundaries, idempotency, wrong after streak.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Define algorithm spec
- [ ] Implement pure policy
- [ ] Persistence
- [ ] Golden tests.

## 8. Dependencies & Boundaries

**Blocked by:** [CONTENT-001](../content/CONTENT-001-vocabulary-catalog-metadata-schema-jsonb.md), [AUTH-003](../identity/AUTH-003-tao-chinh-sua-ho-so-tre-toi-a-5.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Khoảng cách ôn (ngày), decay và score weights chưa chốt trong PRD.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
