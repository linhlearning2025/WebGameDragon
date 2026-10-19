---
id: GAME-008
title: "Speaking — quyền micro, capture và fallback"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: gameplay
dependencies: [GAME-004]
source: "PRD US-3.5; Mic HUD"
stack: [react, phaser-3, dotnet, postgresql]
---

# GAME-008 — Speaking — quyền micro, capture và fallback

> **Spec-driven ticket** · Domain: **Mini-games & Reward** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn đọc từ cho Rồng phun lửa mà hiểu rõ khi mic chưa sẵn sàng.

**Nguồn:** PRD US-3.5; Mic HUD. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Yêu cầu microphone sau user gesture với parent-approved context và giải thích đơn giản.
- Hiển thị listening/not-listening, timeout và không thu âm nền khi scene rời.
- Nếu Web Speech API không hỗ trợ hoặc denied, có lộ trình fallback không ghi mastery là phát âm đúng.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given browser hỗ trợ mic
When bé bấm Start
Then hiển thị mic active chỉ khi permission granted.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given permission denied
When start
Then hướng dẫn thử lại hoặc chuyển chế độ thay thế.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given back/home
When exit
Then dừng recognition stream.
```

## 4. Contract & Data Impact

- Browser SpeechRecognition adapter; event `dragon:mic-permission-status`; optional audio never uploaded by default.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Secure context HTTPS bắt buộc; iOS/Android/browser support thay đổi.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Permission denied, unsupported, background exit, consent audit.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Speech adapter
- [ ] Mic indicator
- [ ] Cleanup/fallback
- [ ] Cross-browser tests.

## 8. Dependencies & Boundaries

**Blocked by:** [GAME-004](../gameplay/GAME-004-khung-mini-game-chung-trang-thai-va-phan-hoi-tich-cuc.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt fallback học tập khi browser không hỗ trợ; cần đánh giá privacy trẻ em và parental consent.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
