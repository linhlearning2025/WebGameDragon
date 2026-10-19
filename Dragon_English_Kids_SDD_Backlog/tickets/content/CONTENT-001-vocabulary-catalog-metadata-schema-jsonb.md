---
id: CONTENT-001
title: "Vocabulary catalog + metadata schema JSONB"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M0
domain: content
dependencies: [PLAT-003]
source: "PRD §3.5"
stack: [react, phaser-3, dotnet, postgresql]
---

# CONTENT-001 — Vocabulary catalog + metadata schema JSONB

> **Spec-driven ticket** · Domain: **Vocabulary & AI Content** · Priority: **P0** · Release: **M0** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là content editor, tôi cần lưu mỗi từ Cambridge với phân loại và media nhất quán.

**Nguồn:** PRD §3.5. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Word, tier STARTERS|MOVERS, topic, difficulty 1–4 và URL media có validation.
- Metadata định nghĩa schema/version: phonics[], distractor_ids[], acceptable_pronunciations[] (không bao giờ expose như đáp án scoring thô).
- Mỗi vocab có publishing status để game chỉ dùng published media.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given vocab thiếu image/audio
When publish
Then validator từ chối.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given distractor_id trỏ từ không tồn tại
When validate
Then trả lỗi dữ liệu.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given unknown tier
When save
Then fail validation.
```

## 4. Contract & Data Impact

- vocabularies PRD + proposed `content_status`, `metadata_schema_version`; JSON Schema tại contracts.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Malformed JSONB: validation ở API/tool; tránh cho vào stage selection.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] JSON schema tests, orphan refs, missing URL, publish gates.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Design metadata schema
- [ ] EF validation
- [ ] Publish rules
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [PLAT-003](../platform/PLAT-003-postgresql-schema-migrations-va-domain-constraints.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt ID stable khi import, licensing Cambridge list; difficulty 4 nội dung khác placement 1–3.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
