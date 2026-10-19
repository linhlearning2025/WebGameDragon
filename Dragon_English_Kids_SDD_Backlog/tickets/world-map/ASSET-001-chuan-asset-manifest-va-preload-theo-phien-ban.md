---
id: ASSET-001
title: "Chuẩn asset manifest và preload theo phiên bản"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: world-map
dependencies: [PLAT-001]
source: "Yêu cầu asset pack; PRD §1 Game Canvas"
stack: [react, phaser-3, dotnet, postgresql]
---

# ASSET-001 — Chuẩn asset manifest và preload theo phiên bản

> **Spec-driven ticket** · Domain: **World Map & Assets** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là Phaser dev, tôi muốn load asset qua manifest thay vì hard-code đường dẫn PNG.

**Nguồn:** Yêu cầu asset pack; PRD §1 Game Canvas. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Manifest mapping keys → URL, logical size, pivot/origin, category, version/checksum.
- Fallback sprite khi thiếu asset; kiểm soát cache invalidation theo asset version.
- Định nghĩa atlas/spritesheet nếu frame animation cần dùng; UI text không bake vào background.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given manifest mới có thêm island
When preload
Then scene render key mới không sửa code.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given key không tồn tại
When render
Then dùng placeholder được log, không crash.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given cache version thay đổi
When reload
Then asset mới được tải.
```

## 4. Contract & Data Impact

- `public/assets/game/manifest.json` proposed schema; key `island.animals`, `node.listening.available`, `dragon.red.idle`.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Manifest/API version mismatch: dùng compatible schema hoặc fail có màn thông báo.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Manifest schema validation, 404 fallback, version change test.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Asset inventory
- [ ] Manifest schema
- [ ] Loader plugin
- [ ] Fallback tests.

## 8. Dependencies & Boundaries

**Blocked by:** [PLAT-001](../platform/PLAT-001-thiet-lap-monorepo-va-moi-truong-phat-trien.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Xác minh bộ PNG thực tế: frame spritesheet và layer thực sự độc lập, không dùng preview flatten để thay thế.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
