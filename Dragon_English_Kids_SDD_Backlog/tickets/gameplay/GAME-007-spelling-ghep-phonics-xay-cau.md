---
id: GAME-007
title: "Spelling — ghép phonics xây cầu"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: gameplay
dependencies: [GAME-004, CONTENT-001]
source: "PRD US-3.4"
stack: [react, phaser-3, dotnet, postgresql]
---

# GAME-007 — Spelling — ghép phonics xây cầu

> **Spec-driven ticket** · Domain: **Mini-games & Reward** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn chạm các viên chữ theo thứ tự để xây cầu.

**Nguồn:** PRD US-3.4. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Phonics từ metadata có thứ tự và có thể chứa multi-character graphemes; không tách bằng từng ký tự Unicode tùy tiện.
- Mỗi input hợp lệ tạo một đoạn cầu; hỗ trợ undo/clear trước submit và âm thanh khích lệ.
- Trộn tile bảo đảm mỗi token được dùng đúng số lần kể cả chữ lặp.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given phonics ["sh","i","p"]
When chọn đúng
Then xây đủ 3 đoạn cầu và complete.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given apple có 2 ký tự p
When shuffle tiles
Then có 2 tile p độc lập.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given chọn sai
When check
Then feedback nhẹ nhàng và không đánh dấu mastery correct.
```

## 4. Contract & Data Impact

- Skill DTO `{phonicsTokens:[{tileId,text}],imageUrl,promptAudioUrl}`; server owns canonical answer.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Missing phonics metadata: stage question không được tạo, báo content issue.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Repeated token, Unicode grapheme, undo, shuffle determinism.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Tile layout
- [ ] Bridge animation
- [ ] Answer validation
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [GAME-004](../gameplay/GAME-004-khung-mini-game-chung-trang-thai-va-phan-hoi-tich-cuc.md), [CONTENT-001](../content/CONTENT-001-vocabulary-catalog-metadata-schema-jsonb.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt scoring theo chữ hay grapheme; PRD dùng phonics nhưng ví dụ metadata là ký tự.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
