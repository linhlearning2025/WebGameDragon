---
id: GAME-006
title: "Reading — chọn từ phá đá"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: gameplay
dependencies: [GAME-004]
source: "PRD US-3.3"
stack: [react, phaser-3, dotnet, postgresql]
---

# GAME-006 — Reading — chọn từ phá đá

> **Spec-driven ticket** · Domain: **Mini-games & Reward** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn chọn chữ đúng với hình để Rồng phá tảng đá.

**Nguồn:** PRD US-3.3. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Hiển thị hình rõ, 2–4 text options; lời nhắc bằng icon/audio khi cần.
- Text đáp án có font dễ đọc, khoảng cách chạm phù hợp, tránh chỉ phân biệt bằng màu.
- Correct answer kích hoạt phá đá; incorrect dùng hint tích cực, không làm mất lượt do animation.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given hình cat
When chọn “cat”
Then đá vỡ và question hoàn thành.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given chọn “cap”
When feedback
Then highlight word nhẹ nhàng và cho retry/next theo rule.
```

## 4. Contract & Data Impact

- Skill DTO `{imageUrl,choices:[{choiceId,text}]}`; server validates choice IDs.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Text overflow/language special chars: dynamic sizing, ellipsis only if unambiguous.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Answer mapping, fonts, touch, long text.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Reading scene
- [ ] Stone break animation
- [ ] Question validation
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [GAME-004](../gameplay/GAME-004-khung-mini-game-chung-trang-thai-va-phan-hoi-tich-cuc.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt word length và số distractors cho lứa tuổi 5–8.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-02 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
