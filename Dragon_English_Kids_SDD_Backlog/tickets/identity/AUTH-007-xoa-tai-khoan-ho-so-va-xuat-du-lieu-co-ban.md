---
id: AUTH-007
title: "Xóa tài khoản/hồ sơ và xuất dữ liệu cơ bản"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P1
milestone: M3
domain: identity
dependencies: [AUTH-003, PLAT-005]
source: "PRD đối tượng trẻ em; yêu cầu privacy bổ sung"
stack: [react, phaser-3, dotnet, postgresql]
---

# AUTH-007 — Xóa tài khoản/hồ sơ và xuất dữ liệu cơ bản

> **Spec-driven ticket** · Domain: **Identity & Child Profiles** · Priority: **P1** · Release: **M3** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là phụ huynh, tôi muốn kiểm soát và xóa dữ liệu học tập của gia đình.

**Nguồn:** PRD đối tượng trẻ em; yêu cầu privacy bổ sung. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Xóa/ẩn profile có bước xác nhận gate; dữ liệu liên quan SRS, dragon, stage progress được xử lý có cascade policy.
- Không xóa bằng GET, hỗ trợ recovery window nếu quy định.
- Cho parent truy xuất báo cáo dữ liệu profile dưới định dạng tải về.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given parent đủ quyền
When xóa profile
Then profile không thể chơi/hiển thị.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given user khác
When xóa profile không sở hữu
Then không ảnh hưởng dữ liệu.
```

## 4. Contract & Data Impact

- Proposed `DELETE /api/v1/parent/children/{id}`, `GET .../export`; delete workflow.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Xử lý dữ liệu trong R2/backup theo retention policy.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Cascade/orphan scan, ownership test, export smoke.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Data inventory
- [ ] Delete/export API
- [ ] Gate confirmation
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [AUTH-003](../identity/AUTH-003-tao-chinh-sua-ho-so-tre-toi-a-5.md), [PLAT-005](../platform/PLAT-005-kiem-soat-truy-cap-theo-phu-huynh-tre-va-khach.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt retention, recovery, legal compliance theo thị trường phân phối.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-02 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
