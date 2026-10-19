---
id: QA-001
title: "Đặc tả accessibility và UX an toàn cho trẻ 5–8"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: quality
dependencies: [PLAT-001]
source: "PRD target children; no red X"
stack: [react, phaser-3, dotnet, postgresql]
---

# QA-001 — Đặc tả accessibility và UX an toàn cho trẻ 5–8

> **Spec-driven ticket** · Domain: **Quality, Privacy & Release** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé với khả năng đọc/điều khiển khác nhau, tôi muốn game dễ hiểu và ít gây nản.

**Nguồn:** PRD target children; no red X. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Nút chạm đủ lớn (mục tiêu ≥44x44 CSS px), tương phản và icon kèm nhãn/voice cue.
- Không dùng biểu tượng X đỏ cho đáp án sai; feedback tích cực và tránh timer áp lực theo game mode.
- Tôn trọng reduced motion, có mute, replay audio và lỗi có hướng dẫn bằng hình.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given tablet/mobile
When thao tác
Then các target chính dễ bấm và không overlap.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given reduced-motion enabled
When mở scenes
Then không ép animation liên tục.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given âm thanh bị mute
When hiển thị instruction
Then không chỉ dựa audio để hướng dẫn.
```

## 4. Contract & Data Impact

- UX acceptance matrix cho 360x800, 768x1024, desktop; design tokens/audio prompt.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Audio-only task khi mute: hiển thị lựa chọn khôi phục nghe, không đánh dấu sai.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Touch target audit, visual contrast, reduced motion, screen-reader DOM parent UI.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] UX checklist
- [ ] Design tokens
- [ ] Cross-device review
- [ ] Automated checks.

## 8. Dependencies & Boundaries

**Blocked by:** [PLAT-001](../platform/PLAT-001-thiet-lap-monorepo-va-moi-truong-phat-trien.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Giáo dục và accessibility review phải sign-off theo target user study.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
