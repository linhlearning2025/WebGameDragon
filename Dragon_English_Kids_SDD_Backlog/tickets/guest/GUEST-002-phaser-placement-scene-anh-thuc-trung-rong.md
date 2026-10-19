---
id: GUEST-002
title: "Phaser Placement Scene “Đánh thức Trứng Rồng”"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: guest
dependencies: [GUEST-001, GUEST-003, PLAT-002]
source: "PRD US-2.2"
stack: [react, phaser-3, dotnet, postgresql]
---

# GUEST-002 — Phaser Placement Scene “Đánh thức Trứng Rồng”

> **Spec-driven ticket** · Domain: **Guest Trial & Placement** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn làm bài thử như một trò chơi trứng nở, không bị áp lực khi sai.

**Nguồn:** PRD US-2.2. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Phaser hiện trứng, câu hỏi, nút nghe, đáp án hình/âm; 6–8 câu cho một trial.
- Trứng nứt dần khi tiến triển; không hiển thị dấu X đỏ hoặc ngôn ngữ phạt.
- Tích hợp preloaded media và pause/restore khi tab ẩn.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given trial mới
When mở scene
Then trứng nguyên và câu hỏi đầu được phát/hiển thị.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given trả lời sai
When feedback
Then nhẹ nhàng động viên, không dấu X đỏ.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given hoàn tất số câu hợp lệ
When scene kết thúc
Then chuyển sang hatching.
```

## 4. Contract & Data Impact

- Phaser Scene `PlacementTestScene`; event `dragon:placement-answer`; backend answer endpoint tùy hợp đồng GUEST-003.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Asset/audio fail: fallback hình/text, cho thử lại mà không mất lượt.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] UI flow E2E 5–8, no-negative-feedback assertions, loss of focus.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Render questions
- [ ] Egg animation
- [ ] Answer bridge
- [ ] Scene tests.

## 8. Dependencies & Boundaries

**Blocked by:** [GUEST-001](../guest/GUEST-001-khoi-tao-phien-guest-va-chon-o-tuoi.md), [GUEST-003](../guest/GUEST-003-ngan-hang-cau-hoi-placement-va-ghi-nhan-cau-tra-loi.md), [PLAT-002](../platform/PLAT-002-event-bridge-react-phaser-co-kieu-du-lieu.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt animation assets/timing và số câu xác định bởi adaptive engine.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
