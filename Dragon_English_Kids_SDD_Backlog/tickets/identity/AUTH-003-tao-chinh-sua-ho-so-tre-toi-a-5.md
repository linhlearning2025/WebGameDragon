---
id: AUTH-003
title: "Tạo/chỉnh sửa hồ sơ trẻ tối đa 5"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: identity
dependencies: [AUTH-001]
source: "PRD US-1.2; §3.3"
stack: [react, phaser-3, dotnet, postgresql]
---

# AUTH-003 — Tạo/chỉnh sửa hồ sơ trẻ tối đa 5

> **Spec-driven ticket** · Domain: **Identity & Child Profiles** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là phụ huynh, tôi muốn quản lý tối đa 5 hồ sơ học tập riêng.

**Nguồn:** PRD US-1.2; §3.3. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Nickname bắt buộc; birth_month 1–12 và birth_year hợp lệ; không yêu cầu tên thật.
- Tối đa 5 active child profiles mỗi parent, kiểm tra dưới concurrent requests.
- ChildId UUID do server cấp, progress/dragon tách biệt hoàn toàn.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given parent có 4 hồ sơ
When thêm 1
Then thành công.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given parent có 5 hồ sơ
When thêm
Then nhận business code `CHILD_LIMIT_REACHED`.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given 2 yêu cầu tạo đồng thời khi đang có 4
When hoàn tất
Then tổng không vượt 5.
```

## 4. Contract & Data Impact

- Proposed `/api/v1/parent/children` GET/POST, `/{id}` PATCH.
- child_profiles schema PRD + unique owner indexes.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Nickname quá dài/HTML: validation và escape khi render.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Boundary birth values, concurrent create, ownership checks.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] API create/list/update
- [ ] React forms
- [ ] Concurrency & authorization tests.

## 8. Dependencies & Boundaries

**Blocked by:** [AUTH-001](../identity/AUTH-001-ang-ky-ang-nhap-phu-huynh-bang-email.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt có cho phép xóa/khôi phục hồ sơ; yêu cầu dữ liệu dưới tuổi theo chính sách.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
