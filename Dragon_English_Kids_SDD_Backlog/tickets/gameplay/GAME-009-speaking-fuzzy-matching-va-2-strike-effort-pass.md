---
id: GAME-009
title: "Speaking — fuzzy matching và 2-strike effort-pass"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: gameplay
dependencies: [GAME-008, GAME-002]
source: "PRD US-3.5 fuzzy, 2-Strike Auto-Pass"
stack: [react, phaser-3, dotnet, postgresql]
---

# GAME-009 — Speaking — fuzzy matching và 2-strike effort-pass

> **Spec-driven ticket** · Domain: **Mini-games & Reward** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi không muốn bị phạt vì phát âm chưa rõ; hệ thống vẫn phân biệt nỗ lực với phát âm chính xác.

**Nguồn:** PRD US-3.5 fuzzy, 2-Strike Auto-Pass. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Chuẩn hóa transcript theo locale, từ cho phép/phonetics có version; kết quả CORRECT, RETRY, EFFORT_PASS, UNAVAILABLE.
- Hai lần đọc có nỗ lực đủ điều kiện có thể EFFORT_PASS để đi tiếp; không ghi CORRECT hoặc tăng mastery như đúng.
- “Đủ âm lượng” không suy ra đáng tin chỉ từ transcript; nếu dùng loudness cần sự đồng ý và đo local có threshold được test.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given lần 1 không đạt
When trả lời
Then khuyến khích thử lần 2.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given lần 2 vẫn lệch nhưng effort đủ
When evaluate
Then cho qua với EFFORT_PASS, mastery không tăng như correct.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given API mic unavailable
When fallback
Then có outcome UNAVAILABLE, không giả định bé đã phát âm.
```

## 4. Contract & Data Impact

- Proposed `SpeechAttempt{attemptNo,transcript?,confidence?,effortEvidence?}`; `SpeakingOutcome` enum.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Nhiễu môi trường/mic không nhận: không coi là lack of effort.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Test accent variants, false positives, two attempts, unavailable and mastery mapping.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Scoring policy
- [ ] Effort classifier
- [ ] Outcome mapping
- [ ] Golden corpus tests.

## 8. Dependencies & Boundaries

**Blocked by:** [GAME-008](../gameplay/GAME-008-speaking-quyen-micro-capture-va-fallback.md), [GAME-002](../gameplay/GAME-002-ghi-nhan-ket-qua-stage-idempotent-va-chong-gian-lan.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Fuzzy thresholds/acceptable_pronunciations cần chuyên gia ngôn ngữ kiểm tra; 2-strike là UX advance không phải chứng chỉ năng lực.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
