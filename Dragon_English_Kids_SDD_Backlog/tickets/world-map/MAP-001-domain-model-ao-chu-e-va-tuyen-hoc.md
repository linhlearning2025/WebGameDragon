---
id: MAP-001
title: "Domain model đảo, chủ đề và tuyến học"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: world-map
dependencies: [CONTENT-001, PLAT-003]
source: "PRD §2 World Map; Cambridge topics"
stack: [react, phaser-3, dotnet, postgresql]
---

# MAP-001 — Domain model đảo, chủ đề và tuyến học

> **Spec-driven ticket** · Domain: **World Map & Assets** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là content designer, tôi muốn cấu hình các đảo/chủ đề/level bằng dữ liệu backend, không chỉnh sprite source.

**Nguồn:** PRD §2 World Map; Cambridge topics. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Mỗi island có stable ID, topic, tier, order, assetKey; mỗi stage có skill và vị trí x/y trong world coordinates.
- Một island chứa nhiều stage; stage tham chiếu vocabulary selection rule hoặc content set, không copy dữ liệu media.
- Không publish map có node trùng stageId, missing island hoặc đường dẫn asset sai.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given map version published
When truy vấn
Then nhận islands/stages theo order xác định.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given thêm island mới qua admin seed
When fetch map
Then Phaser render island mới qua manifest.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given stage tham chiếu island không tồn tại
When publish
Then validation thất bại.
```

## 4. Contract & Data Impact

- Proposed DB `islands`, `stages`, `map_versions`, `stage_prerequisites`; `GET /api/v1/game/{childId}/world-map`.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Map thiếu configuration: trả version cuối hợp lệ, không fabricate học phần.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Referential constraints, ordering, asset key validation.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] World domain entities
- [ ] Map seed/schema
- [ ] Config validation
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [CONTENT-001](../content/CONTENT-001-vocabulary-catalog-metadata-schema-jsonb.md), [PLAT-003](../platform/PLAT-003-postgresql-schema-migrations-va-domain-constraints.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt các đảo Cambridge cụ thể: Animals, Food, Colors, Family, School... chỉ là đề xuất.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
