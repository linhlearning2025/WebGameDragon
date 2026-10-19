---
id: GUEST-005
title: "Nở trứng, tạo Rồng guest và tùy chỉnh"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: guest
dependencies: [GUEST-002, GUEST-004]
source: "PRD US-2.4; §2 user flow"
stack: [react, phaser-3, dotnet, postgresql]
---

# GUEST-005 — Nở trứng, tạo Rồng guest và tùy chỉnh

> **Spec-driven ticket** · Domain: **Guest Trial & Placement** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn nhận Rồng con sau bài thử và đặt tên/chọn màu.

**Nguồn:** PRD US-2.4; §2 user flow. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Hoàn thành placement hợp lệ tạo một guest dragon state với evolution_level=1.
- Cho chọn từ palette màu được asset pack hỗ trợ; dragonName dùng filter/maxlength.
- Hatching popup nhắc ba mẹ đăng ký để lưu tiến trình; không ép trẻ khai email.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given placement done
When hatch
Then hiển thị baby dragon cùng màu và tên đã chọn.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given refresh trước claim
When quay lại trong TTL
Then giữ cùng Rồng, không nhân đôi.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given invalid name
When submit
Then yêu cầu sửa với ngôn ngữ nhẹ nhàng.
```

## 4. Contract & Data Impact

- Guest result proposed `{dragonName, colorId, tier, testSummary}`; guest JSONB temporary dragon.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Không có asset màu chọn: dùng default có chủ đích, không crash.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Refresh/retry idempotency, name validation, missing asset.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Hatching scene
- [ ] Customization
- [ ] Persist guest result
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [GUEST-002](../guest/GUEST-002-phaser-placement-scene-anh-thuc-trung-rong.md), [GUEST-004](../guest/GUEST-004-thuat-toan-placement-adaptive-va-phan-tang.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt danh sách màu/sprite hỗ trợ từ pack và lọc tên không phù hợp.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
