---
id: GAME-002
title: "Ghi nhận kết quả stage idempotent và chống gian lận"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: gameplay
dependencies: [GAME-001, MAP-002, SRS-001]
source: "PRD POST stage-complete"
stack: [react, phaser-3, dotnet, postgresql]
---

# GAME-002 — Ghi nhận kết quả stage idempotent và chống gian lận

> **Spec-driven ticket** · Domain: **Mini-games & Reward** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là hệ thống, tôi muốn lưu kết quả học một lần đúng thứ tự, tránh client tự cộng EXP và sao.

**Nguồn:** PRD POST stage-complete. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Stage session được cấp server-side, signed/opaque, có TTL và childId/stageId.
- Client gửi item answers/attempts; backend tính correctness dựa answer keys hoặc signed server-evaluated attempts.
- Completion transaction ghi results + SRS + progress + reward ledger; idempotency key không thưởng lặp.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given stage session hợp lệ
When submit
Then lưu kết quả và trả stars/rewards/unlock.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given replay same completion key
When submit
Then response giống lần đầu, không cộng thưởng lặp.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given fabricated stageId/answers
When submit
Then từ chối không thay đổi state.
```

## 4. Contract & Data Impact

- `POST /api/v1/game/{childId}/stage-complete`; request `{stageSessionId,idempotencyKey,answers[]}`; response progress/rewards.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Timeout sau commit: retry safe; partial failure rollback.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Forged payload, concurrent duplicate, transactional consistency, TTL expiry.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Session storage
- [ ] Completion handler
- [ ] Transaction/ledger
- [ ] Security tests.

## 8. Dependencies & Boundaries

**Blocked by:** [GAME-001](../gameplay/GAME-001-stage-data-api-tron-70-moi-30-on-srs.md), [MAP-002](../world-map/MAP-002-quy-tac-mo-khoa-va-luu-stage-progress.md), [SRS-001](../srs/SRS-001-domain-srs-mastery-streak-va-lich-on.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Cần chốt speech server trust model nếu Web Speech API chỉ cung cấp transcript từ client.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
