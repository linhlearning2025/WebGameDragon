---
id: DRAGON-001
title: "Hang Rồng render state và điều hướng"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: dragon
dependencies: [AUTH-003, ASSET-001]
source: "PRD US-3.1"
stack: [react, phaser-3, dotnet, postgresql]
---

# DRAGON-001 — Hang Rồng render state và điều hướng

> **Spec-driven ticket** · Domain: **Dragon Lair & Evolution** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn vào Hang Rồng để xem Rồng, EXP, mood và inventory.

**Nguồn:** PRD US-3.1. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Một child có một dragon state; tạo default khi child được tạo/claim nếu chưa có.
- Lair hiển thị tên, skin, current EXP/evolution level/mood và food; không sửa state chỉ qua UI.
- Lair hoạt động với dữ liệu cached read-only khi API lỗi, nhưng hành động feed yêu cầu server online.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given child có dragon
When vào lair
Then hiển thị đúng trạng thái.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given dragon state missing do legacy
When GET
Then server tạo/repair idempotent theo policy.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given API unavailable
When xem
Then hiện offline indicator và khóa feed.
```

## 4. Contract & Data Impact

- Proposed `GET /api/v1/game/{childId}/dragon` → state DTO; dragon_states PRD.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Tuyệt đối không tạo dragon thứ hai khi request đồng thời.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Default state, ownership, offline read-only, responsive lair.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Read endpoint
- [ ] Lair UI
- [ ] Fallback
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [AUTH-003](../identity/AUTH-003-tao-chinh-sua-ho-so-tre-toi-a-5.md), [ASSET-001](../world-map/ASSET-001-chuan-asset-manifest-va-preload-theo-phien-ban.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt initial inventory và tâm trạng mặc định.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
