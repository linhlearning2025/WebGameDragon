---
id: QA-002
title: "Chính sách dữ liệu trẻ em, consent và bảo vệ thông tin"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M0
domain: quality
dependencies: [PLAT-001]
source: "PRD guest, parent, mic; privacy cần bổ sung"
stack: [react, phaser-3, dotnet, postgresql]
---

# QA-002 — Chính sách dữ liệu trẻ em, consent và bảo vệ thông tin

> **Spec-driven ticket** · Domain: **Quality, Privacy & Release** · Priority: **P0** · Release: **M0** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là phụ huynh, tôi muốn biết dữ liệu nào được thu thập và khi nào trẻ dùng micro.

**Nguồn:** PRD guest, parent, mic; privacy cần bổ sung. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Data minimization: hạn chế DOB chi tiết, nickname thay real name; phân loại PII.
- Guest token TTL/deletion, consent flow của phụ huynh cho mic và AI vendor nếu bắt buộc; không upload audio mặc định.
- Encryption in transit/at rest theo infrastructure; retention/privacy request và access logging tối thiểu.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given parent chưa đồng ý mic processing policy
When Speaking start
Then không ghi âm/upload dữ liệu ngoài quyền đã cấp.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given guest hết hạn
When retention job chạy
Then temp_test_result được xóa theo policy.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given log audit
When kiểm tra
Then không lộ child identifier với thông tin nhạy cảm không cần thiết.
```

## 4. Contract & Data Impact

- Privacy inventory, consent record schema, retention policies, vendor DPA checklist.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Unsupported jurisdictions: không tự tuyên bố tuân thủ COPPA/GDPR-K mà chưa legal review.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Consent denial, retention cleanup, redaction, access tests.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Data map
- [ ] Consent design
- [ ] Retention worker
- [ ] Privacy verification.

## 8. Dependencies & Boundaries

**Blocked by:** [PLAT-001](../platform/PLAT-001-thiet-lap-monorepo-va-moi-truong-phat-trien.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Phê duyệt pháp lý theo nơi phát hành; audio stream do browser/vendor Web Speech xử lý cần khảo sát.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
