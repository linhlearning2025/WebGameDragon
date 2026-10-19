---
id: CONTENT-003
title: "Console Tool C# sinh nội dung AI có kiểm duyệt"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P1
milestone: M2
domain: content
dependencies: [CONTENT-001, CONTENT-002]
source: "PRD US-4.1"
stack: [react, phaser-3, dotnet, postgresql]
---

# CONTENT-003 — Console Tool C# sinh nội dung AI có kiểm duyệt

> **Spec-driven ticket** · Domain: **Vocabulary & AI Content** · Priority: **P1** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là content admin, tôi muốn sinh metadata/đáp án gợi ý hàng loạt nhưng không xuất bản nội dung sai tự động.

**Nguồn:** PRD US-4.1. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Job input theo vocabulary/topic và version prompt/model; output có schema validation và trace không chứa secrets.
- AI gợi ý phonics, distractors, prompt/sentence; reviewer duyệt trước publish.
- Có retry exponential backoff, giới hạn cost, resumable per-vocab jobs và failure report.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given batch 100 words
When job chạy và 3 lỗi
Then 97 candidates lưu, 3 lỗi có thể retry.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given AI output sai schema
When validate
Then quarantine và không publish.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given rerun cùng job/version
When execute
Then không tạo duplicate media/metadata.
```

## 4. Contract & Data Impact

- C# CLI `content generate --tier STARTERS --topic Animals --dry-run`; proposed jobs table + JSON output.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Provider timeout/rate-limit: resumable, không hỏng dữ liệu hiện hữu.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Stub provider, malformed JSON, batch recovery, budget cap.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Batch job design
- [ ] Provider adapter
- [ ] Schema validator
- [ ] Reviewer export.

## 8. Dependencies & Boundaries

**Blocked by:** [CONTENT-001](../content/CONTENT-001-vocabulary-catalog-metadata-schema-jsonb.md), [CONTENT-002](../content/CONTENT-002-bo-du-lieu-chu-e-starters-movers-va-kiem-soat-trung.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt model/provider, bản quyền đầu ra, human approval policy.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
