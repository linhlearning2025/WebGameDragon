---
id: ASSET-002
title: "World Map responsive với camera và hit area"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: world-map
dependencies: [ASSET-001, PLAT-002]
source: "PRD Map; asset 2D cartoon; responsive"
stack: [react, phaser-3, dotnet, postgresql]
---

# ASSET-002 — World Map responsive với camera và hit area

> **Spec-driven ticket** · Domain: **World Map & Assets** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé dùng mobile hoặc desktop, tôi muốn bản đồ đảo rồng rõ và bấm được.

**Nguồn:** PRD Map; asset 2D cartoon; responsive. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- World coordinates độc lập viewport; camera FIT landscape, pan/zoom có giới hạn trên mobile portrait.
- Tách các đảo/path/decor khỏi level node UI, giữ consistent hit areas ≥44 CSS px tương đương theo scale.
- Cho phép điều hướng bằng touch/mouse và có focus alternative cho accessibility.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given viewport desktop 16:9
When mở map
Then không méo hoặc cắt nội dung quan trọng.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given mobile portrait
When mở map
Then user có thể pan tới mọi island/node.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given resize/rotate
When layout lại
Then current selected level giữ trạng thái.
```

## 4. Contract & Data Impact

- Phaser `WorldMapScene` đọc positions từ world map API, camera bounds/map dimensions từ config.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Texture load chậm: loading skeleton + retry, không cho click sai level.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Visual snapshot 360x800, 768x1024, 1920x1080; touch hitboxes and rotation.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Layout config
- [ ] Camera controls
- [ ] Resize tests
- [ ] Accessibility fallback.

## 8. Dependencies & Boundaries

**Blocked by:** [ASSET-001](../world-map/ASSET-001-chuan-asset-manifest-va-preload-theo-phien-ban.md), [PLAT-002](../platform/PLAT-002-event-bridge-react-phaser-co-kieu-du-lieu.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt orientation ưu tiên và tọa độ bản đồ từ designer.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
