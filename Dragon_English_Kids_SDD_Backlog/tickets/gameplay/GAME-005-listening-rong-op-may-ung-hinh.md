---
id: GAME-005
title: "Listening — Rồng đớp mây đúng hình"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: gameplay
dependencies: [GAME-004]
source: "PRD US-3.2"
stack: [react, phaser-3, dotnet, postgresql]
---

# GAME-005 — Listening — Rồng đớp mây đúng hình

> **Spec-driven ticket** · Domain: **Mini-games & Reward** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn nghe một từ và chọn hình đúng để Rồng ăn mồi.

**Nguồn:** PRD US-3.2. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Phát normal audio, có nút replay và slow audio; 2–4 đám mây hình theo độ tuổi/difficulty.
- Đáp án nhiễu được server tạo từ distractor_ids và tránh duplicate.
- Khi chọn, Rồng bay tới mây; kết quả đúng/sai gửi câu trả lời có evidence.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given audio “apple”
When chọn hình apple
Then nhận positive feedback và ghi correct.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given chọn sai
When feedback
Then phát động viên rồi cho next/retry theo rule.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given âm chưa tải
When play
Then loading state, không tự tính bé sai.
```

## 4. Contract & Data Impact

- Skill DTO `{questionId,promptAudioNormal,promptAudioSlow,choices:[{choiceId,imageUrl}]}`.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Autoplay bị chặn: yêu cầu user tap play trước, không tính timeout.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Audio controls, answer mapping, slow replay, missing media.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Listening scene
- [ ] Cloud choices
- [ ] Animations
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [GAME-004](../gameplay/GAME-004-khung-mini-game-chung-trang-thai-va-phan-hoi-tich-cuc.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt số choice theo tier và retry policy.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
