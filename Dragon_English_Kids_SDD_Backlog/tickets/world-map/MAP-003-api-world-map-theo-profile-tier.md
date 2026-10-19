---
id: MAP-003
title: "API world map theo profile + tier"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: world-map
dependencies: [MAP-001, MAP-002, PLAT-005]
source: "PRD current tier/map data; yêu cầu dynamic backend"
stack: [react, phaser-3, dotnet, postgresql]
---

# MAP-003 — API world map theo profile + tier

> **Spec-driven ticket** · Domain: **World Map & Assets** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là Phaser scene, tôi muốn lấy cấu hình bản đồ và tiến trình cho từng bé bằng một request.

**Nguồn:** PRD current tier/map data; yêu cầu dynamic backend. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Endpoint trả mapVersion, worldSize, islands[], stages[] (state/stars), dragonVisual, assetsVersion.
- Child ownership check; etag/version được dùng cache nội dung không nhạy cảm, state riêng từng child không cache chia sẻ.
- Không expose câu trả lời vocab hoặc thông tin parent trong map DTO.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given child thuộc parent
When GET map
Then trả correct current tier và progress.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given child không thuộc parent
When GET
Then 403/404 theo policy.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given stage progress vừa cập nhật
When refresh
Then dữ liệu mới phản ánh unlock.
```

## 4. Contract & Data Impact

- Proposed `GET /api/v1/game/{childId}/world-map`; response example tại `contracts/world-map.sample.json`.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Chưa có published map: 409 MAP_NOT_CONFIGURED thay vì empty success giả.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] OpenAPI/contract schema, ETag caching, IDOR, progress freshness.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Endpoint DTO
- [ ] Query projection
- [ ] Cache policy
- [ ] Contract tests.

## 8. Dependencies & Boundaries

**Blocked by:** [MAP-001](../world-map/MAP-001-domain-model-ao-chu-e-va-tuyen-hoc.md), [MAP-002](../world-map/MAP-002-quy-tac-mo-khoa-va-luu-stage-progress.md), [PLAT-005](../platform/PLAT-005-kiem-soat-truy-cap-theo-phu-huynh-tre-va-khach.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- API này bổ sung so với PRD để tránh hard-code bản đồ.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
