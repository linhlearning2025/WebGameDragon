---
id: CONTENT-002
title: "Bộ dữ liệu chủ đề STARTERS/MOVERS và kiểm soát trùng"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: content
dependencies: [CONTENT-001]
source: "PRD tổng quan ~750 từ Cambridge"
stack: [react, phaser-3, dotnet, postgresql]
---

# CONTENT-002 — Bộ dữ liệu chủ đề STARTERS/MOVERS và kiểm soát trùng

> **Spec-driven ticket** · Domain: **Vocabulary & AI Content** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là chuyên gia học liệu, tôi muốn nhập danh mục từ và chủ đề đúng chuẩn, đo coverage.

**Nguồn:** PRD tổng quan ~750 từ Cambridge. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Nhập danh sách từ có nguồn và phiên bản bản quyền được phép sử dụng.
- Chuẩn hóa casing, homograph/meaning, topic mapping; tránh duplicate bởi `(tier,normalizedWord,sense)` policy.
- Lập report tổng số từ theo tier/topic và trạng thái publish; ~750 chỉ là mục tiêu quy mô.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given import same content twice
When sync
Then không tạo duplicate.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given từ nằm trong nhiều topic
When truy vấn
Then mapping hợp lệ và ổn định.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given thiếu quyền nội dung/media
When audit
Then không publish.
```

## 4. Contract & Data Impact

- CSV import spec proposed: word,tier,topic,difficulty,phonics,source,rights.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Unknown phonics: vào queue review, không tự fabricated.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Dedup tests, coverage audit, rights metadata check.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Import dataset
- [ ] Dedup normalization
- [ ] Coverage report
- [ ] Review process.

## 8. Dependencies & Boundaries

**Blocked by:** [CONTENT-001](../content/CONTENT-001-vocabulary-catalog-metadata-schema-jsonb.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Cần PO xác nhận corpus chính thức và phạm vi sử dụng danh sách Cambridge.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
