---
id: CONTENT-005
title: "Quy trình review/publish học liệu và asset"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P1
milestone: M2
domain: content
dependencies: [CONTENT-004]
source: "PRD US-4.1; yêu cầu chất lượng bổ sung"
stack: [react, phaser-3, dotnet, postgresql]
---

# CONTENT-005 — Quy trình review/publish học liệu và asset

> **Spec-driven ticket** · Domain: **Vocabulary & AI Content** · Priority: **P1** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là content reviewer, tôi muốn chỉ phát hành câu hỏi/âm thanh/hình đã được kiểm tra.

**Nguồn:** PRD US-4.1; yêu cầu chất lượng bổ sung. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Workflow DRAFT→IN_REVIEW→PUBLISHED→ARCHIVED, actor+time+version audit.
- Kiểm tra audio/image URL, nội dung age-appropriate, distractor chất lượng và consistency phonics.
- Rollback version đã publish mà không đổi ID logic của vocabulary.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given draft thiếu audio slow
When publish
Then báo thiếu field.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given reviewer duyệt đủ
When publish
Then stage API chỉ chọn version published.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given archive vocab
When stage generation
Then không được chọn nội dung archived.
```

## 4. Contract & Data Impact

- Proposed `PATCH /api/v1/admin/vocabularies/{id}/status`; restricted admin RBAC.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Review reject không ghi đè bản published hiện dùng trong active session.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] RBAC tests, stale version, publish gating, rollback.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Publishing state machine
- [ ] RBAC
- [ ] Validation
- [ ] Audit tests.

## 8. Dependencies & Boundaries

**Blocked by:** [CONTENT-004](../content/CONTENT-004-tts-am-thanh-thuong-cham-va-upload-cloudflare-r2.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Admin UI có thể là MVP giản lược qua CLI; chốt reviewer và quyền.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
