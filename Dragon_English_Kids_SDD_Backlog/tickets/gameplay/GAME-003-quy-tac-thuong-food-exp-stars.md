---
id: GAME-003
title: "Quy tắc thưởng food, EXP, stars"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: gameplay
dependencies: [GAME-002]
source: "PRD US-3.1; stage-complete; reward"
stack: [react, phaser-3, dotnet, postgresql]
---

# GAME-003 — Quy tắc thưởng food, EXP, stars

> **Spec-driven ticket** · Domain: **Mini-games & Reward** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn nhận phần thưởng có thể dự đoán và công bằng sau khi học.

**Nguồn:** PRD US-3.1; stage-complete; reward. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Scoring server-side từ answer outcomes, rule version: stars 0–3, food quantities, earned EXP.
- Reward event ledger unique per completion session; replay có thể luyện tập nhưng reward abuse giới hạn config.
- Auto-pass speaking có outcome khác `correct`, phải không tính mastery như đáp án đúng.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given 80% correct
When stage ends
Then reward mapping theo table versioned (sẽ được PO duyệt).
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given same session replay API
When retry
Then inventory và XP không tăng lần hai.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given speaking effort-pass
When calculate learning accuracy
Then không ghi là phonetic correct.
```

## 4. Contract & Data Impact

- Proposed `reward_rules`, `reward_ledger`, `completion_outcomes`; API response `{stars,foodRewards,expEarned}`.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- DB inconsistency: transactional rollback all affected aggregates.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Rule matrix, idempotency, concurrency, mastering separation tests.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Score calculator
- [ ] Ledger persistence
- [ ] Reward feedback
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [GAME-002](../gameplay/GAME-002-ghi-nhan-ket-qua-stage-idempotent-va-chong-gian-lan.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chưa có score thresholds/reward amounts trong PRD — cần quyết định sản phẩm.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
