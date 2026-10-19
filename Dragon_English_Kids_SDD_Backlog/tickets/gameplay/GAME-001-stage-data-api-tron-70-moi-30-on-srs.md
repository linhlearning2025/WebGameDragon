---
id: GAME-001
title: "Stage-data API trộn 70% mới / 30% ôn SRS"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: gameplay
dependencies: [CONTENT-002, SRS-002, MAP-002]
source: "PRD US-4.2; GET stage-data"
stack: [react, phaser-3, dotnet, postgresql]
---

# GAME-001 — Stage-data API trộn 70% mới / 30% ôn SRS

> **Spec-driven ticket** · Domain: **Mini-games & Reward** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là mini-game, tôi cần bộ câu hỏi phù hợp stage, có từ mới và từ ôn tập.

**Nguồn:** PRD US-4.2; GET stage-data. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Server chọn từ published theo tier/topic/difficulty, khoảng 70% new + 30% due SRS.
- Khi đủ pool, chia theo deterministic rounding; khi thiếu phải fallback documented, không lặp từ trong cùng stage.
- Trả itemId, answer choices, image/audio URLs, playback variants; không expose correctAnswer plaintext trước submit.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given 10 questions và đủ pool
When fetch
Then 7 new + 3 due review.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given due review chỉ có 1
When fetch
Then fill bằng eligible new và annotate composition.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given stage locked
When fetch
Then bị từ chối.
```

## 4. Contract & Data Impact

- `GET /api/v1/game/{childId}/stage-data?stageId=...`; proposed `stageSessionId` và composition info.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- No eligible published vocab: 409 STAGE_CONTENT_UNAVAILABLE.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Distribution cases n=7/8/10, due shortage, duplicate and access tests.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Selection engine
- [ ] API projection
- [ ] Media DTO
- [ ] Contract tests.

## 8. Dependencies & Boundaries

**Blocked by:** [CONTENT-002](../content/CONTENT-002-bo-du-lieu-chu-e-starters-movers-va-kiem-soat-trung.md), [SRS-002](../srs/SRS-002-query-tu-en-han-va-phan-phoi-pool-theo-tier.md), [MAP-002](../world-map/MAP-002-quy-tac-mo-khoa-va-luu-stage-progress.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- PRD không quy định stage length hay rounding; đề xuất 10 questions per stage cần approval.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
